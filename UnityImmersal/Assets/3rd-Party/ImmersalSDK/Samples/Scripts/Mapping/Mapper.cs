/*===============================================================================
Copyright (C) 2020 Immersal Ltd. All Rights Reserved.

This file is part of the Immersal SDK.

The Immersal SDK cannot be copied, distributed, or made available to
third-parties for commercial purposes without written permission of Immersal Ltd.

Contact sdk@immersal.com for licensing requests.
===============================================================================*/

using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Immersal.AR;
using Immersal.REST;
using Immersal.Samples.Util;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Immersal.Samples.Mapping
{
    public class Mapper : MapperBase
    {
        private void SessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            ImageRunUpdate();

            m_IsTracking = args.state == ARSessionState.SessionTracking;

            var captureButton = workspaceManager.captureButton.GetComponent<Button>();
            var localizeButton = visualizeManager.localizeButton.GetComponent<Button>();
            captureButton.interactable = m_IsTracking;
            localizeButton.interactable = m_IsTracking;
        }

        protected override void OnEnable()
        {
#if !UNITY_EDITOR
            ARSession.stateChanged += SessionStateChanged;
#endif
            base.OnEnable();
        }

        protected override void OnDisable()
        {
#if !UNITY_EDITOR
            ARSession.stateChanged -= SessionStateChanged;
#endif
            base.OnDisable();
        }

        protected override async void Capture(bool anchor)
        {
            await Task.Delay(250);

            m_bCaptureRunning = true;
            float captureStartTime = Time.realtimeSinceStartup;
            float uploadStartTime = Time.realtimeSinceStartup;

#if PLATFORM_LUMIN && UNITY_2020_1
            XRCameraImage image;
#else
            XRCpuImage image;
#endif
            ARCameraManager cameraManager = m_Sdk.cameraManager;
            var cameraSubsystem = cameraManager.subsystem;

#if PLATFORM_LUMIN && UNITY_2020_1
            if (cameraSubsystem != null && cameraSubsystem.TryGetLatestImage(out image))
#else
            if (cameraSubsystem != null && cameraSubsystem.TryAcquireLatestCpuImage(out image))
#endif
            {
                JobCaptureAsync j = new JobCaptureAsync();
                j.run = (int)(m_ImageRun & 0xEFFFFFFF);
                j.index = m_ImageIndex++;
                j.anchor = anchor;

                if (mapperSettings.useGps)
                {
                    j.latitude = m_Latitude;
                    j.longitude = m_Longitude;
                    j.altitude = m_Altitude;
                }
                else
                {
                    j.latitude = j.longitude = j.altitude = 0.0;
                }

                Camera cam = this.mainCamera;
                Quaternion _q = cam.transform.rotation;
                Matrix4x4 rot = Matrix4x4.Rotate(new Quaternion(_q.x, _q.y, -_q.z, -_q.w));
                Vector3 _p = cam.transform.position;
                Vector3 pos = new Vector3(_p.x, _p.y, -_p.z);
                j.rotation = rot;
                j.position = pos;

				ARHelper.GetIntrinsics(out j.intrinsics);

                int width = image.width;
                int height = image.height;

                byte[] pixels;
                int channels = 1;

                if (mapperSettings.captureRgb)
                {
                    ARHelper.GetPlaneDataRGB(out pixels, image);
                    channels = 3;
                }
                else
                {
                    ARHelper.GetPlaneData(out pixels, image);
                }

                byte[] capture = new byte[channels * width * height + 1024];
                int useMatching = mapperSettings.checkConnectivity ? 1 : 0;

                Task<icvCaptureInfo> captureTask = Task.Run(() =>
                {
                    return Core.CaptureImage(capture, capture.Length, pixels, width, height, channels, useMatching);
                });

                await captureTask;

                string path = string.Format("{0}/{1}", this.tempImagePath, System.Guid.NewGuid());
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path)))
                {
                    writer.Write(capture, 0, captureTask.Result.captureSize);
                }

                j.imagePath = path;
                j.encodedImage = "";

                if (mapperSettings.checkConnectivity)
                {
                    NotifyIfConnected(captureTask.Result);
                }

                if (m_SessionFirstImage)
                    m_SessionFirstImage = false;
                
                j.OnStart += () =>
                {
                    uploadStartTime = Time.realtimeSinceStartup;
                    mappingUIManager.SetProgress(0);
                    mappingUIManager.ShowProgressBar();
                };
                j.OnResult += (SDKImageResult result) =>
                {
                    float et = Time.realtimeSinceStartup - uploadStartTime;
                    Debug.Log(string.Format("Image uploaded successfully in {0} seconds", et));

                    mappingUIManager.HideProgressBar();
                };
                j.Progress.ProgressChanged += (s, progress) =>
                {
                    int value = (int)(100f * progress);
                    //Debug.Log(string.Format("Upload progress: {0}%", value));
                    mappingUIManager.SetProgress(value);
                };
                j.OnError += (e) =>
                {
                    mappingUIManager.HideProgressBar();
                };

                m_Jobs.Add(j.RunJobAsync());
                image.Dispose();

                float elapsedTime = Time.realtimeSinceStartup - captureStartTime;
                Debug.Log(string.Format("Capture in {0} seconds", elapsedTime));
            }

            m_bCaptureRunning = false;
            var captureButton = workspaceManager.captureButton.GetComponent<Button>();
            captureButton.interactable = true;
        }

        public void TryLocalize()
        {
            if (mapperSettings.useServerLocalizer)
            {
                int n = ARSpace.mapIdToMap.Count;
                SDKMapId[] mapIds = new SDKMapId[n];

                int count = 0;
                foreach (int id in ARSpace.mapIdToMap.Keys)
                {
                    mapIds[count] = new SDKMapId();
                    mapIds[count++].id = id;
                }

                if (m_UseGeoPose)
                {
                    m_Sdk.Localizer.LocalizeGeoPose(mapIds);
                }
                else
                {
                    m_Sdk.Localizer.LocalizeServer(mapIds);
                }
            }
            else
            {
                m_Sdk.Localizer.Localize();
            }
        }
    }
}
