using UnityEngine;
using System.Collections;

/****************************   정보   ****************************

    스카이박스 매니저

    스카이박스와 라이팅을 바꿔 준다.

    사용방법 :

    1. 매니저 오브젝트에 추가 후 컴포넌트 연결 및 설정
    2. 사용하기 위해서는 ChackAreaForSkyboxObject 오브젝트를 이용해야 한다
    
******************************************************************/

public class SkyBoxMgr : MonoBehaviour {
    [System.Serializable]
    public struct EnvironmentLighting
    {
        public Color SkyColor;
        public Color EquatorColor;
        public Color GroundColor;
        [Range(0.0f, 8.0f)]
        public float Intensity;
    }

    public float durationTime;
    public Light mainLight;
    public Light[] DirectionalLights;
    public EnvironmentLighting[] AmbientSettings;
    public Material[] skies;

    private Skybox skyBox;
    private float blend = 0f;

    public static SkyBoxMgr instance;    // 싱글톤

    void Start () {
        instance = this;
        
        /*
        RenderSettings.ambientSkyColor = AmbientSettings[0].SkyColor;
        RenderSettings.ambientEquatorColor = AmbientSettings[0].EquatorColor;
        RenderSettings.ambientGroundColor = AmbientSettings[0].GroundColor;
        RenderSettings.ambientIntensity = AmbientSettings[0].Intensity;

        */
        RenderSettings.skybox = skies[0];
        RenderSettings.skybox.SetFloat("_Blend", 0);
        RenderSettings.skybox.SetColor("_FogColor", RenderSettings.fogColor);
        
    }

    // 스카이 박스와 라이팅 변경 ( 외부 호출 )
    public void SwapSkyBoxAndLight(int skyboxNumber, Dir dir)
    {
        Debug.Log(111);
        RenderSettings.skybox = skies[skyboxNumber];
        StopAllCoroutines();
        StartCoroutine(SwapLight(skyboxNumber, dir));
        StartCoroutine(SwapSkyBox_Cor(dir));
    }

    // 스카이 박스 스왑
    IEnumerator SwapSkyBox_Cor(Dir dir)
    {
        float time = 0;

        //  오른쪽으로 갈때
        while (Dir.RIGHT == dir && time < 1)
        {
            time += Time.smoothDeltaTime / durationTime;
            blend = Mathf.Lerp(blend, 1f, time);
            RenderSettings.skybox.SetFloat("_Blend", blend);

            if (blend >= 0.99)
            {
                break;
            }
            yield return null;
        }
        
        // 왼쪽으로 갈때
        while (Dir.LEFT == dir && time < 1)
        {
            time += Time.smoothDeltaTime / durationTime;
            blend = Mathf.Lerp(blend, 0f, time);
            RenderSettings.skybox.SetFloat("_Blend", blend);
            
            if (blend <= 0.01)
            {
                break;
            }
            yield return null;      
        }
    }

    // 라이팅 스왑
    IEnumerator SwapLight(int skyboxNumber, Dir dir)
    { 
        int dirVal, lightIdx;
        float time = 0;

        if (Dir.LEFT == dir)
            dirVal = 0;
        else
            dirVal = 1;

        lightIdx = skyboxNumber + dirVal;

        while (time < 1)
        {
            time += Time.smoothDeltaTime / durationTime;
            // 각도 변경
            mainLight.transform.rotation = Quaternion.Lerp(
                mainLight.transform.rotation, DirectionalLights[lightIdx].transform.rotation, time);

            // 색 변경
            mainLight.color = Color.Lerp(mainLight.color, DirectionalLights[lightIdx].color, time);

            // 빛 세기 변경
            mainLight.intensity = Mathf.Lerp(mainLight.intensity, DirectionalLights[lightIdx].intensity, time);
            /*
            // 엠비언트
            RenderSettings.ambientSkyColor = 
                 Color.Lerp(RenderSettings.ambientSkyColor,AmbientSettings[lightIdx].SkyColor,time);
            RenderSettings.ambientEquatorColor =
                 Color.Lerp(RenderSettings.ambientEquatorColor,AmbientSettings[lightIdx].EquatorColor,time);
            RenderSettings.ambientGroundColor =
                 Color.Lerp(RenderSettings.ambientGroundColor,AmbientSettings[lightIdx].GroundColor,time);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, AmbientSettings[lightIdx].Intensity, time);
            */
            yield return null;
        }
    }

}
