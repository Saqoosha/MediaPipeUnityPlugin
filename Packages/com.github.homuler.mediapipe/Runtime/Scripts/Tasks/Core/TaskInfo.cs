// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Mediapipe.Tasks.Core
{
  internal class TaskInfo<T> where T : ITaskOptions
  {
    public string taskGraph { get; }
    public List<string> inputStreams { get; }
    public List<string> outputStreams { get; }
    public List<string> inputSidePackets { get; }
    public T taskOptions { get; }

    public TaskInfo(string taskGraph, List<string> inputStreams, List<string> outputStreams, List<string> inputSidePackets, T taskOptions)
    {
      this.taskGraph = taskGraph;
      this.inputStreams = inputStreams;
      this.outputStreams = outputStreams;
      this.inputSidePackets = inputSidePackets;
      this.taskOptions = taskOptions;
    }

    public CalculatorGraphConfig GenerateGraphConfig(bool enableFlowLimiting = false)
    {
      if (string.IsNullOrEmpty(taskGraph) || taskOptions == null)
      {
        throw new InvalidOperationException("Please provide both `task_graph` and `task_options`.");
      }
      if (inputStreams?.Count <= 0 || outputStreams?.Count <= 0)
      {
        throw new InvalidOperationException("Both `input_streams` and `output_streams` must be non-empty.");
      }

      if (!enableFlowLimiting)
      {
        return new CalculatorGraphConfig()
        {
          Node = {
            new CalculatorGraphConfig.Types.Node()
            {
              Calculator = taskGraph,
              Options = taskOptions.ToCalculatorOptions(),
              InputStream = { inputStreams },
              OutputStream = { outputStreams },
              InputSidePacket = { inputSidePackets },
            },
          },
          InputStream = { inputStreams },
          OutputStream = { outputStreams },
          InputSidePacket = { inputSidePackets },
        };
      }

      var throttledInputStreams = inputStreams.Select(AddStreamNamePrefix);
      var finishedStream = $"FINISHED:{Tool.ParseNameFromStream(outputStreams.First())}";
      var flowLimiterOptions = new CalculatorOptions();
      flowLimiterOptions.SetExtension(FlowLimiterCalculatorOptions.Extensions.Ext, new FlowLimiterCalculatorOptions()
      {
        MaxInFlight = 1,
        MaxInQueue = 1,
      });

      return new CalculatorGraphConfig()
      {
        Node = {
          new CalculatorGraphConfig.Types.Node()
          {
            Calculator = "FlowLimiterCalculator",
            InputStreamInfo = {
              new InputStreamInfo()
              {
                TagIndex = "FINISHED",
                BackEdge = true,
              },
            },
            InputStream = { inputStreams.Select(Tool.ParseNameFromStream).Append(finishedStream) },
            OutputStream = { throttledInputStreams.Select(Tool.ParseNameFromStream) },
            InputSidePacket = { inputSidePackets },
            Options = flowLimiterOptions,
          },
          new CalculatorGraphConfig.Types.Node()
          {
            Calculator = taskGraph,
            InputStream = { throttledInputStreams },
            OutputStream = { outputStreams },
            InputSidePacket = { inputSidePackets },
            Options = taskOptions.ToCalculatorOptions(),
          },
        },
        InputStream = { inputStreams },
        OutputStream = { outputStreams },
        InputSidePacket = { inputSidePackets },
      };
    }

    private static string AddStreamNamePrefix(string tagIndexName)
    {
      Tool.ParseTagAndName(tagIndexName, out var tag, out var name);
      return $"{tag}:throttled_{name}";
    }
  }
}
