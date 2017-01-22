using UnityEngine;
using System.Collections;

public class Wobble : MonoBehaviour {

	public float wobbleHeight = 0.2f;
	public float wobbleSpeed = 1f;
	public float wobbleDensity = 1f;
	public bool recalculateNormals = true;
	public int seed;
	Vector3[] originalVerts;
	Vector3[] originalNormals;
	Mesh m;
	Vector3 time;
	int vertLength;
	Vector3 vector3One = Vector3.one;
	Vector3[] vertices;

	void Start () {
		Random.seed = seed;	//Allows multiple meshes to be synchronized
		m = GetComponent<MeshFilter>().mesh;
		m.MarkDynamic();
		originalVerts = m.vertices;
		originalNormals = m.normals;
		time = new Vector3(Random.Range(0, 60), Random.Range(0, 60), Random.Range(0, 60));
		vertLength = originalVerts.Length;
		vertices = new Vector3[vertLength];
    }

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
			wobbleSpeed *= -1;
		time +=  vector3One * wobbleSpeed * Time.deltaTime;

		for (int i = 0; i < vertLength; i++) {
			Vector3 baseVert = time + (originalVerts[i] * wobbleDensity);

            vertices[i] = originalVerts[i] + (
				Mathf.PerlinNoise(baseVert.x, baseVert.y) +
				Mathf.PerlinNoise(baseVert.z, baseVert.y) +
				Mathf.PerlinNoise(baseVert.x, baseVert.z) - 
				1.5f
			) * originalNormals[i] * wobbleHeight / 2;
		}

		m.vertices = vertices;
		if (recalculateNormals)
			m.RecalculateNormals();
	}
}
