using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour {

    private Matrix4x4 mat = new Matrix4x4();

    public static void TransformFromMatrix(Matrix4x4 matrix, Transform trans)
    {
        trans.rotation = QuaternionFromMatrix(matrix);
        Vector3 tmp = matrix.GetColumn(3); // uses implicit conversion from Vector4 to Vector3
        // We need to invert the translation on the X axis
        tmp.x = -tmp.x;
        trans.position = tmp;
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));

        // We need to invert rotations on Y and Z axis
        q.y = -q.y;
        q.z = -q.z;

        return q;
    }

	// Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 16; ++i)
            mat[i] = (float)background.transMat[i];

        if (!mat.Equals(Matrix4x4.zero))
            TransformFromMatrix(mat, this.transform);
    }
}
