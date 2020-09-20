using System.Collections.Generic;
using UnityEngine;

using Edge = System.Tuple<int ,int>;

namespace Mediapipe {
  public class FaceLandmarkListAnnotationController : MonoBehaviour {
    private static readonly IList<Edge> connections = new List<Edge> {
      // Face Oval
      new Edge(10, 338),
      new Edge(338, 297),
      new Edge(297, 332),
      new Edge(332, 284),
      new Edge(284, 251),
      new Edge(251, 389),
      new Edge(389, 356),
      new Edge(356, 454),
      new Edge(454, 323),
      new Edge(323, 361),
      new Edge(361, 288),
      new Edge(288, 397),
      new Edge(397, 365),
      new Edge(365, 379),
      new Edge(379, 378),
      new Edge(378, 400),
      new Edge(400, 377),
      new Edge(377, 152),
      new Edge(152, 148),
      new Edge(148, 176),
      new Edge(176, 149),
      new Edge(149, 150),
      new Edge(150, 136),
      new Edge(136, 172),
      new Edge(172, 58),
      new Edge(58, 132),
      new Edge(132, 93),
      new Edge(93, 234),
      new Edge(234, 127),
      new Edge(127, 162),
      new Edge(162, 21),
      new Edge(21, 54),
      new Edge(54, 103),
      new Edge(103, 67),
      new Edge(67, 109),
      new Edge(109, 10),
      // Left Eye
      new Edge(33, 7),
      new Edge(7, 163),
      new Edge(163, 144),
      new Edge(144, 145),
      new Edge(145, 153),
      new Edge(153, 154),
      new Edge(154, 155),
      new Edge(155, 133),
      new Edge(33, 246),
      new Edge(246, 161),
      new Edge(161, 160),
      new Edge(160, 159),
      new Edge(159, 158),
      new Edge(158, 157),
      new Edge(157, 173),
      new Edge(173, 133),
      // Left Eyebrow
      new Edge(46, 53), 
      new Edge(53, 52), 
      new Edge(52, 65),
      new Edge(65, 55),
      new Edge(70, 63),
      new Edge(63, 105),
      new Edge(105, 66),
      new Edge(66, 107),
      // Right Eye
      new Edge(263, 249),
      new Edge(249, 390),
      new Edge(390, 373),
      new Edge(373, 374),
      new Edge(374, 380),
      new Edge(380, 381),
      new Edge(381, 382),
      new Edge(382, 362),
      new Edge(263, 466),
      new Edge(466, 388),
      new Edge(388, 387),
      new Edge(387, 386),
      new Edge(386, 385),
      new Edge(385, 384),
      new Edge(384, 398),
      new Edge(398, 362),
      // Right Eyebrow
      new Edge(276, 283),
      new Edge(283, 282),
      new Edge(282, 295),
      new Edge(295, 285),
      new Edge(300, 293),
      new Edge(293, 334),
      new Edge(334, 296),
      new Edge(296, 336),
      // Lips (Inner)
      new Edge(78, 95),
      new Edge(95, 88),
      new Edge(88, 178),
      new Edge(178, 87),
      new Edge(87, 14),
      new Edge(14, 317),
      new Edge(317, 402),
      new Edge(402, 318),
      new Edge(318, 324),
      new Edge(324, 308),
      new Edge(78, 191),
      new Edge(191, 80),
      new Edge(80, 81),
      new Edge(81, 82),
      new Edge(82, 13),
      new Edge(13, 312),
      new Edge(312, 311),
      new Edge(311, 310),
      new Edge(310, 415),
      new Edge(415, 308),
      // Lips (Outer)
      new Edge(61, 146),
      new Edge(146, 91),
      new Edge(91, 181),
      new Edge(181, 84),
      new Edge(84, 17),
      new Edge(17, 314),
      new Edge(314, 405),
      new Edge(405, 321),
      new Edge(321, 375),
      new Edge(375, 291),
      new Edge(61, 185),
      new Edge(185, 40),
      new Edge(40, 39),
      new Edge(39, 37),
      new Edge(37, 0),
      new Edge(0, 267),
      new Edge(267, 269),
      new Edge(269, 270),
      new Edge(270, 409),
      new Edge(409, 291),
    };

    private static readonly int NodeSize = 468;
    private static readonly int EdgeSize = connections.Count;

    [SerializeField] GameObject nodePrefab = null;
    [SerializeField] GameObject edgePrefab = null;

    private List<GameObject> nodes = new List<GameObject>(NodeSize);
    private List<GameObject> edges = new List<GameObject>(EdgeSize);

    void Awake() {
      for (var i = 0; i < NodeSize; i++) {
        nodes.Add(Instantiate(nodePrefab));
      }

      for (var i = 0; i < EdgeSize; i++) {
        edges.Add(Instantiate(edgePrefab));
      }
    }

    public void Clear() {
      foreach (var landmark in nodes) {
        landmark.GetComponent<NodeAnnotationController>().Clear();
      }

      foreach (var line in edges) {
        line.GetComponent<EdgeAnnotationController>().Clear();
      }
    }

    /// <summary>
    ///   Renders face landmarks on a screen.
    ///   It is assumed that the screen vertical to terrain and not inverted.
    /// </summary>
    /// <param name="isFlipped">
    ///   if true, x axis is oriented from right to left (top-right point is (0, 0) and bottom-left is (1, 1))
    /// </param>
    /// <remarks>
    ///   In <paramref name="landmarkList" />, y-axis is oriented from top to bottom.
    /// </remarks>
    public void Draw(Transform screenTransform, NormalizedLandmarkList landmarkList, bool isFlipped = false) {
      var localScale = screenTransform.localScale;
      var scale = new Vector3(10 * localScale.x, 10 * localScale.z, 1);

      for (var i = 0; i < NodeSize; i++) {
        var landmark = landmarkList.Landmark[i];
        var node = nodes[i];

        node.GetComponent<NodeAnnotationController>().Draw(screenTransform, landmark, isFlipped, 0.3f);
      }

      for (var i = 0; i < EdgeSize; i++) {
        var connection = connections[i];
        var edge = edges[i];

        var a = nodes[connection.Item1];
        var b = nodes[connection.Item2];

        edge.GetComponent<EdgeAnnotationController>().Draw(screenTransform, a, b);
      }
    }
  }
}
