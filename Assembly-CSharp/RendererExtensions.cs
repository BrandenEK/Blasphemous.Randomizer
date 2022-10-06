using System;
using UnityEngine;

public static class RendererExtensions
{
	public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
	{
		GeometryUtility.CalculateFrustumPlanes(camera, RendererExtensions.planeBuffer);
		return GeometryUtility.TestPlanesAABB(RendererExtensions.planeBuffer, renderer.bounds);
	}

	private static readonly Plane[] planeBuffer = new Plane[6];
}
