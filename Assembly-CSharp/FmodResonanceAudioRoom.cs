using System;
using UnityEngine;

[AddComponentMenu("ResonanceAudio/FmodResonanceAudioRoom")]
public class FmodResonanceAudioRoom : MonoBehaviour
{
	private void OnEnable()
	{
		FmodResonanceAudio.UpdateAudioRoom(this, FmodResonanceAudio.IsListenerInsideRoom(this));
	}

	private void OnDisable()
	{
		FmodResonanceAudio.UpdateAudioRoom(this, false);
	}

	private void Update()
	{
		FmodResonanceAudio.UpdateAudioRoom(this, FmodResonanceAudio.IsListenerInsideRoom(this));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, this.size);
	}

	public FmodResonanceAudioRoom.SurfaceMaterial leftWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

	public FmodResonanceAudioRoom.SurfaceMaterial rightWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

	public FmodResonanceAudioRoom.SurfaceMaterial floor = FmodResonanceAudioRoom.SurfaceMaterial.ParquetOnConcrete;

	public FmodResonanceAudioRoom.SurfaceMaterial ceiling = FmodResonanceAudioRoom.SurfaceMaterial.PlasterRough;

	public FmodResonanceAudioRoom.SurfaceMaterial backWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

	public FmodResonanceAudioRoom.SurfaceMaterial frontWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

	public float reflectivity = 1f;

	public float reverbGainDb;

	public float reverbBrightness;

	public float reverbTime = 1f;

	public Vector3 size = Vector3.one;

	public enum SurfaceMaterial
	{
		Transparent,
		AcousticCeilingTiles,
		BrickBare,
		BrickPainted,
		ConcreteBlockCoarse,
		ConcreteBlockPainted,
		CurtainHeavy,
		FiberglassInsulation,
		GlassThin,
		GlassThick,
		Grass,
		LinoleumOnConcrete,
		Marble,
		Metal,
		ParquetOnConcrete,
		PlasterRough,
		PlasterSmooth,
		PlywoodPanel,
		PolishedConcreteOrTile,
		Sheetrock,
		WaterOrIceSurface,
		WoodCeiling,
		WoodPanel
	}
}
