using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*************************   정보   **************************
    
    날씨 매니저
    옵저버 패턴으로 구현 했음
    
    
    사용방법 :

    1.  WeatherSubject WeatherData 이런식으로 변수 선언

    2.  WeatherData = WeatherMgr.GetInstance().RetrunThis();
        WeatherData.registerObserver(this);
        이렇게 옵버저 등록을 한다

    3.  날씨 영향을 받을 오브젝트에 WeatherObserver를 상속 받아
        updateObserver 를 통해 업데이트 받음

    ★ 매니저가 자동 생성 되므로 오브젝트에 추가하지 말자 ★ 

*************************************************************/

// 날씨의 종류
public enum WeatherState
{
    NONE = 0x1, WIND_LR = 0x2, WIND_UD = 0x4, RAIN = 0x8,
}

public interface WeatherObserver
{
    void updateObserver(WeatherState state, float value);
}

public interface WeatherSubject
{
    void registerObserver(WeatherObserver o);
    void removeObserver(WeatherObserver o);
    void notifyObservers();
}

public partial class WeatherMgr : MonoBehaviour, WeatherSubject
{
    List<WeatherObserver> observers;
    private WeatherState weatherState;
    private float weatherValue;
    bool isWinding = false;

    // 옵저버 추가
    public void registerObserver(WeatherObserver o)
    {
        observers.Add(o);
    }

    // 옵저버 삭제
    public void removeObserver(WeatherObserver o)
    {
        if (observers.Count >= 0)
            observers.Remove(o);
    }

    // 옵저버들에게 통지
    public void notifyObservers()
    {
        WeatherObserver observer;

        for (int i = 0; i < observers.Count; i++)
        {
            observer = observers[i];
            observer.updateObserver(weatherState, weatherValue);
        }
    }

    // 날씨 입력
    public void SetWeather(WeatherState weatherState, float weatherValue)
    {
        this.weatherState = weatherState;
        this.weatherValue = weatherValue;
        changeWeather();
    }

    // 날씨 변경
    public void changeWeather()
    {
        notifyObservers();
    }

    // 날씨 효과 초기화
    IEnumerator WeatherReset(float timer)
    {
        yield return new WaitForSeconds(timer);
        SetWeather(WeatherState.NONE, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeather(WeatherState.WIND_LR, 4);
            StartCoroutine(WeatherReset(3.0f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeather(WeatherState.RAIN, 4);
            StartCoroutine(WeatherReset(4.0f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeather(WeatherState.WIND_LR | WeatherState.RAIN, 4);
            StartCoroutine(WeatherReset(4.0f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetWeather(WeatherState.WIND_UD,4);
            StartCoroutine(WeatherReset(8.0f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetWeather(WeatherState.NONE, 0);
        }
    }
}

// 싱글톤 / 초기화
public partial class WeatherMgr : MonoBehaviour, WeatherSubject
{
    private static WeatherMgr instance;
    private static GameObject container;
    public static WeatherMgr GetInstance()
    {
        if (!instance)
        {
            container = new GameObject();
            container.name = "WeatherMgr";
            instance = container.AddComponent(typeof(WeatherMgr)) as WeatherMgr;

        }
        return instance;
    }

    public WeatherMgr()
    {
        observers = new List<WeatherObserver>();
    }

    public WeatherMgr RetrunThis()
    {
        return this;
    }
}