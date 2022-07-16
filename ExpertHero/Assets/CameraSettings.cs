using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField(), Range(-20.0f, 20.0f)] private float CameraSet;
    [SerializeField] public CinemachineVirtualCamera Camera;

    [SerializeField] float firstCameraView;
    [SerializeField] float firstCameraBodyOffsetZ;
    [SerializeField] float firstCameraTrackedOffsetY;

    [SerializeField] float cameraView_Iter;
    [SerializeField] float cameraBodyOffsetZ_Iter;
    [SerializeField] float cameraTrackedOffsetY_Iter;
    void Start()
    {
        Camera.m_Lens.FieldOfView = firstCameraView ;
        Camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = firstCameraBodyOffsetZ ;
        Camera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = firstCameraTrackedOffsetY ;
    }
    public void cameraSet(int playerClone, int playerCurrentAmount)
    {
        //levelText.text = (level + 1).ToString();
        //int yearOld = Globals.currentYear;
        //Globals.currentYear = Globals.currentYear + miktar;
        //LeanTween.value(yearOld, Globals.currentYear, 0.2f).setOnUpdate((float val) =>

        float playerAmountOld = (float)playerCurrentAmount;
      float  _playerCurrentAmount = (float)playerCurrentAmount + (float)playerClone;
        LeanTween.value(playerAmountOld, _playerCurrentAmount, 0.5f).setOnUpdate((float val) =>
        {
            CameraSet = val;
            _Update();
        });
    }
    // Update is called once per frame
    void _Update()
    {
        Camera.m_Lens.FieldOfView = firstCameraView + CameraSet * cameraView_Iter;
        Camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = firstCameraBodyOffsetZ + CameraSet * cameraBodyOffsetZ_Iter;
        Camera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = firstCameraTrackedOffsetY + CameraSet * cameraTrackedOffsetY_Iter;
    }
    //void Update()
    //{
    //    Camera.m_Lens.FieldOfView = firstCameraView + CameraSet * cameraView_Iter;
    //    Camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = firstCameraBodyOffsetZ + CameraSet * cameraBodyOffsetZ_Iter;
    //    Camera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = firstCameraTrackedOffsetY + CameraSet * cameraTrackedOffsetY_Iter;
    //}
}
