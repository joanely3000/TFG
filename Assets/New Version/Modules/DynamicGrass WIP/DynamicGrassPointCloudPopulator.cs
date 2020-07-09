﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicGrass
{
	[ExecuteAlways]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class DynamicGrassPointCloudPopulator : MonoBehaviour
	{
		//
		// Other components
		private MeshFilter meshFilter;

		//
		// Editor varaibles
		[Header("Grass")]
		[SerializeField] [Range(0, 65536)] private int grassAmount = 0;
		[SerializeField] [Range(0, 180)] private float slopeThreshold = 45;
		[Header("Region")]
		[SerializeField] private Vector3 boxSize = Vector3.zero;

		//
		// Private variables
		private Mesh mesh;

		//
		// Public variables
		

		//--------------------------
		// MonoBehaviour methods
		//--------------------------
		private void Awake()
		{
			meshFilter = GetComponent<MeshFilter>();
			Populate();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(transform.position, boxSize);
		}

		//--------------------------
		// DynamicGrassPopulator methods
		//--------------------------
		public void Populate()
		{
			Random.InitState((int) (transform.position.x + transform.position.z));
			List<Vector3> vertexPositions = new List<Vector3>();
			List<int> indicies = new List<int>();
			List<Vector3> normals = new List<Vector3>();

			int index = 0;
			for (int i = 0; i < grassAmount; ++i)
			{
				Vector3 origin = transform.position;

				origin.x += boxSize.x * Random.Range(-0.5f, 0.5f);
				origin.z += boxSize.z * Random.Range(-0.5f, 0.5f);
				origin.y += boxSize.y / 2;

				Ray ray = new Ray(origin, Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, boxSize.y))
				{
					DynamicGrassSurface surface = hit.transform.gameObject.GetComponent<DynamicGrassSurface>();
					if (surface != null)
					{
						if (Vector3.Angle(hit.normal, Vector3.up) <= slopeThreshold)
						{
							origin = hit.point;

							vertexPositions.Add(origin - transform.position);
							indicies.Add(index);
							++index;
							normals.Add(hit.normal);
						}
					}
				}
			}

			mesh = new Mesh();
			mesh.SetVertices(vertexPositions);
			mesh.SetIndices(indicies, MeshTopology.Points, 0);
			mesh.SetNormals(normals);
			meshFilter.mesh = mesh;
			Debug.Log("Populated");
		}
	}
}