using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public static class FmodResonanceAudio
{
	public static void UpdateAudioRoom(FmodResonanceAudioRoom room, bool roomEnabled)
	{
		if (roomEnabled)
		{
			if (!FmodResonanceAudio.enabledRooms.Contains(room))
			{
				FmodResonanceAudio.enabledRooms.Add(room);
			}
		}
		else
		{
			FmodResonanceAudio.enabledRooms.Remove(room);
		}
		if (FmodResonanceAudio.enabledRooms.Count > 0)
		{
			FmodResonanceAudioRoom room2 = FmodResonanceAudio.enabledRooms[FmodResonanceAudio.enabledRooms.Count - 1];
			FmodResonanceAudio.RoomProperties roomProperties = FmodResonanceAudio.GetRoomProperties(room2);
			IntPtr intPtr = Marshal.AllocHGlobal(FmodResonanceAudio.roomPropertiesSize);
			Marshal.StructureToPtr(roomProperties, intPtr, false);
			FmodResonanceAudio.ListenerPlugin.setParameterData(FmodResonanceAudio.roomPropertiesIndex, FmodResonanceAudio.GetBytes(intPtr, FmodResonanceAudio.roomPropertiesSize));
			Marshal.FreeHGlobal(intPtr);
		}
		else
		{
			FmodResonanceAudio.ListenerPlugin.setParameterData(FmodResonanceAudio.roomPropertiesIndex, FmodResonanceAudio.GetBytes(IntPtr.Zero, 0));
		}
	}

	public static bool IsListenerInsideRoom(FmodResonanceAudioRoom room)
	{
		VECTOR vector;
		RuntimeManager.LowlevelSystem.get3DListenerAttributes(0, ref FmodResonanceAudio.listenerPositionFmod, ref vector, ref vector, ref vector);
		Vector3 vector2;
		vector2..ctor(FmodResonanceAudio.listenerPositionFmod.x, FmodResonanceAudio.listenerPositionFmod.y, FmodResonanceAudio.listenerPositionFmod.z);
		Vector3 vector3 = vector2 - room.transform.position;
		Quaternion quaternion = Quaternion.Inverse(room.transform.rotation);
		FmodResonanceAudio.bounds.size = Vector3.Scale(room.transform.lossyScale, room.size);
		return FmodResonanceAudio.bounds.Contains(quaternion * vector3);
	}

	private static DSP ListenerPlugin
	{
		get
		{
			if (!FmodResonanceAudio.listenerPlugin.hasHandle())
			{
				FmodResonanceAudio.listenerPlugin = FmodResonanceAudio.Initialize();
			}
			return FmodResonanceAudio.listenerPlugin;
		}
	}

	private static float ConvertAmplitudeFromDb(float db)
	{
		return Mathf.Pow(10f, 0.05f * db);
	}

	private static void ConvertAudioTransformFromUnity(ref Vector3 position, ref Quaternion rotation)
	{
		Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
		matrix4x = FmodResonanceAudio.flipZ * matrix4x * FmodResonanceAudio.flipZ;
		position = matrix4x.GetColumn(3);
		rotation = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
	}

	private static byte[] GetBytes(IntPtr ptr, int length)
	{
		if (ptr != IntPtr.Zero)
		{
			byte[] array = new byte[length];
			Marshal.Copy(ptr, array, 0, length);
			return array;
		}
		return new byte[1];
	}

	private static FmodResonanceAudio.RoomProperties GetRoomProperties(FmodResonanceAudioRoom room)
	{
		Vector3 position = room.transform.position;
		Quaternion rotation = room.transform.rotation;
		Vector3 vector = Vector3.Scale(room.transform.lossyScale, room.size);
		FmodResonanceAudio.ConvertAudioTransformFromUnity(ref position, ref rotation);
		FmodResonanceAudio.RoomProperties result;
		result.positionX = position.x;
		result.positionY = position.y;
		result.positionZ = position.z;
		result.rotationX = rotation.x;
		result.rotationY = rotation.y;
		result.rotationZ = rotation.z;
		result.rotationW = rotation.w;
		result.dimensionsX = vector.x;
		result.dimensionsY = vector.y;
		result.dimensionsZ = vector.z;
		result.materialLeft = room.leftWall;
		result.materialRight = room.rightWall;
		result.materialBottom = room.floor;
		result.materialTop = room.ceiling;
		result.materialFront = room.frontWall;
		result.materialBack = room.backWall;
		result.reverbGain = FmodResonanceAudio.ConvertAmplitudeFromDb(room.reverbGainDb);
		result.reverbTime = room.reverbTime;
		result.reverbBrightness = room.reverbBrightness;
		result.reflectionScalar = room.reflectivity;
		return result;
	}

	private static DSP Initialize()
	{
		int num = 0;
		DSP result = default(DSP);
		Bank[] array = null;
		RuntimeManager.StudioSystem.getBankCount(ref num);
		RuntimeManager.StudioSystem.getBankList(ref array);
		for (int i = 0; i < num; i++)
		{
			int num2 = 0;
			Bus[] array2 = null;
			array[i].getBusCount(ref num2);
			array[i].getBusList(ref array2);
			RuntimeManager.StudioSystem.flushCommands();
			for (int j = 0; j < num2; j++)
			{
				string text = null;
				array2[j].getPath(ref text);
				RuntimeManager.StudioSystem.getBus(text, ref array2[j]);
				RuntimeManager.StudioSystem.flushCommands();
				ChannelGroup channelGroup;
				array2[j].getChannelGroup(ref channelGroup);
				RuntimeManager.StudioSystem.flushCommands();
				if (channelGroup.hasHandle())
				{
					int num3 = 0;
					channelGroup.getNumDSPs(ref num3);
					for (int k = 0; k < num3; k++)
					{
						channelGroup.getDSP(k, ref result);
						int num4 = 0;
						uint num5 = 0U;
						string text2;
						result.getInfo(ref text2, ref num5, ref num4, ref num4, ref num4);
						if (text2.ToString().Equals(FmodResonanceAudio.listenerPluginName) && result.hasHandle())
						{
							return result;
						}
					}
				}
			}
		}
		Debug.LogError(FmodResonanceAudio.listenerPluginName + " not found in the FMOD project.");
		return result;
	}

	public const float maxGainDb = 24f;

	public const float minGainDb = -24f;

	public const float maxReverbBrightness = 1f;

	public const float minReverbBrightness = -1f;

	public const float maxReverbTime = 3f;

	public const float maxReflectivity = 2f;

	private static readonly Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));

	private static readonly string listenerPluginName = "Resonance Audio Listener";

	private static readonly int roomPropertiesSize = Marshal.SizeOf(typeof(FmodResonanceAudio.RoomProperties));

	private static readonly int roomPropertiesIndex = 1;

	private static Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	private static List<FmodResonanceAudioRoom> enabledRooms = new List<FmodResonanceAudioRoom>();

	private static VECTOR listenerPositionFmod = default(VECTOR);

	private static DSP listenerPlugin;

	private struct RoomProperties
	{
		public float positionX;

		public float positionY;

		public float positionZ;

		public float rotationX;

		public float rotationY;

		public float rotationZ;

		public float rotationW;

		public float dimensionsX;

		public float dimensionsY;

		public float dimensionsZ;

		public FmodResonanceAudioRoom.SurfaceMaterial materialLeft;

		public FmodResonanceAudioRoom.SurfaceMaterial materialRight;

		public FmodResonanceAudioRoom.SurfaceMaterial materialBottom;

		public FmodResonanceAudioRoom.SurfaceMaterial materialTop;

		public FmodResonanceAudioRoom.SurfaceMaterial materialFront;

		public FmodResonanceAudioRoom.SurfaceMaterial materialBack;

		public float reflectionScalar;

		public float reverbGain;

		public float reverbTime;

		public float reverbBrightness;
	}
}
