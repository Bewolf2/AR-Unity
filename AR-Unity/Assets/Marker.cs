using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ALVARBridge.alvar_init();
	}

    public static void TransformFromMatrix(Matrix4x4 matrix, Transform trans)
    {
        trans.rotation = QuaternionFromMatrix(matrix);
        trans.position = matrix.GetColumn(3); // uses implicit conversion from Vector4 to Vector3
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
        return q;
    }


	// Update is called once per frame
	void Update () {
        int nb = ALVARBridge.alvar_number_of_detected_markers();
        if (nb > 0)
        {
            double[] transMat = new double[16];
            ALVARBridge.alvar_process(transMat);
            
            Debug.Log(nb + " markers found with matrix={"
                + transMat[0].ToString("F2") + " " + transMat[1].ToString("F2") + " " + transMat[2].ToString("F2") + " " + transMat[3].ToString("F2") + " "
                + transMat[4].ToString("F2") + " " + transMat[5].ToString("F2") + " " + transMat[6].ToString("F2") + " " + transMat[7].ToString("F2") + " "
                + transMat[8].ToString("F2") + " " + transMat[9].ToString("F2") + " " + transMat[10].ToString("F2") + " " + transMat[11].ToString("F2") + " "
                + transMat[12].ToString("F2") + " " + transMat[13].ToString("F2") + " " + transMat[14].ToString("F2") + " " + transMat[15].ToString("F2") + "}");

            Matrix4x4 mat = new Matrix4x4();
            for (int i = 0; i < 16; ++i)
            {
                mat[i] = (float)transMat[i];
            }
            if (!mat.Equals(Matrix4x4.zero))
            {
                TransformFromMatrix(mat, this.transform);
                Debug.Log(this.transform.position.x + " " + this.transform.position.y + " " + this.transform.position.z);
            }
        }
	}
}
