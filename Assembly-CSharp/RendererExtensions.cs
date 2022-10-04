using System;
using UnityEngine;

public static class RendererExtensions
{
	public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
	{
		GeometryUtility.CalculateFrustumPlanes(camera, global::RendererExtensions.planeBuffer);
		return GeometryUtility.TestPlanesAABB(global::RendererExtensions.planeBuffer, renderer.bounds);
	}

	private static readonly Plane[] planeBuffer = new Plane[6];
}
