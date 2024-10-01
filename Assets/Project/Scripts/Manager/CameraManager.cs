using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType { Main, Focus }
public class CameraManager : MonoBehaviour
{
    
    public static CameraManager Instance;

    public CinemachineVirtualCamera[] cameras;

    CinemachineVirtualCamera curCam;

    private void Awake()
    {
        if(Instance == null)Instance = this;
        else Destroy(gameObject);

        cameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        foreach(CinemachineVirtualCamera cam in cameras)
        {
            cam.Priority = 1;
        }
        curCam = cameras[0];
        curCam.Priority = 10;
    }

    public CinemachineVirtualCamera GetCamera(CameraType type)
    {
        // ����ī�޶� �켱�� ���߱�
        curCam.Priority = 1;
        // ī�޶� ��ü
        curCam = cameras[(int)type];
        // ���ο� ī�޶� �켱�� ������
        curCam.Priority = 10;
        return curCam;
    }
}
