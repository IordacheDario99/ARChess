using UnityEngine;

public class GridRenderer : MonoBehaviour 
{
    public static Color DefaultTargetColor = new Color(1.0f, 0.525f, 0, 1.0f);
    private static Color GridColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private static Color MainLineColor = GridColor * 0.9f;
    private static Color UnitLineColor = GridColor * 0.75f;

    public Color TargetColor = DefaultTargetColor;

    private const int NumberOfLinesOnEitherSide = 50;
    private const float LineSpacing = 0.25f;
    private const int IntervalBetweenMainLines = 10;
    private const float TargetSize = 8.0f; 
    
    private Material _lineMaterial;
    private Plane[] _cameraPlanes;

    private void Start() {
        InitializeLineMaterial();
    }

    private void OnRenderObject() {
        // Because the Wikitude SDK uses a secondary camera to render the camera frame, we need to make sure to render the grid only on the main camera.
        if (Camera.current == Camera.main) {
            RenderGrid();
        }
    }

    private void InitializeLineMaterial() {
        var shader = Shader.Find("Hidden/Internal-Colored");
        _lineMaterial = new Material(shader);
        _lineMaterial.hideFlags = HideFlags.HideAndDontSave;

        _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        _lineMaterial.SetInt("_ZWrite", 0);
    }

    private void RenderGrid() {
        _lineMaterial.SetPass(0);
        _cameraPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        GL.Begin(GL.LINES);

        RenderGrid(Vector3.forward, Vector3.right);
        RenderGrid(Vector3.right, Vector3.forward);

        RenderLine(Vector3.forward,  Vector3.zero, TargetColor, TargetSize);
        RenderLine(Vector3.right,  Vector3.zero, TargetColor, TargetSize);

        GL.End();
    }

    private void RenderGrid(Vector3 mainAxis, Vector3 perpendicular) {
        for (int i = -NumberOfLinesOnEitherSide; i <= NumberOfLinesOnEitherSide; ++i) {
            var color = UnitLineColor;
            if (i == 0) {
                color = GridColor;
            } else if (i % IntervalBetweenMainLines == 0) {
                color = MainLineColor;
            }
            float intensity = 1.0f - Mathf.Abs(i * 2.0f) / NumberOfLinesOnEitherSide;
            color.a *= intensity;
            if (color.a > 0.01f) {
                RenderLine(mainAxis, perpendicular * i * LineSpacing, color, (float)NumberOfLinesOnEitherSide / 8.0f);
            }
        }
    }

    private void RenderLine(Vector3 axis, Vector3 offset, Color color, float length) {
        RenderLine(GetTransparent(color), axis * (-length) + offset, color, offset);
        RenderLine(color, offset, GetTransparent(color), axis * length + offset);
    }

    private void RenderLine(Color startColor, Vector3 startPosition, Color endColor, Vector3 endPosition) {
        Color clippedEndColor = Color.Lerp(startColor, endColor, Clip(startPosition, ref endPosition));
        Color clippedStartColor = Color.Lerp(startColor, endColor, 1.0f - Clip(endPosition, ref startPosition));

        GL.Color(clippedStartColor);
        GL.Vertex(startPosition);
        GL.Color(clippedEndColor);
        GL.Vertex(endPosition);
    }

    private float Clip(Vector3 start, ref Vector3 end) {
        // We clip the vertices to the camera planes because on some Android devices clipping of GL_LINES doesn't work correctly.
        Vector3 v = end - start;
        float magnitude = v.magnitude;
        Vector3 vNorm = v / magnitude;

        var startRay = new Ray(start, vNorm);
        float minDistance = magnitude;

        foreach (var plane in _cameraPlanes) {
            if (Vector3.Dot(vNorm, plane.normal) < 0) {
                float enter;
                if (plane.Raycast(startRay, out enter)) {
                    if (enter < minDistance) {
                        minDistance = enter;
                    }
                }
            }
        }

        end = startRay.GetPoint(minDistance);
        return minDistance / magnitude;
    }

    private Color GetTransparent(Color color) {
        color.a = 0.0f;
        return color;
    }
}
