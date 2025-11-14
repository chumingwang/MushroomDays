using UnityEngine;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    public enum Weather { Dry, Rain }

    [Header("Sun")]
    public Light sun;
    [Range(0f, 1f)] public float time01 = 0f;
    public float dayLengthMinutes = 10f;
    public bool runClock = true;
    public AnimationCurve sunIntensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f);
    public Gradient sunColorGradient;

    [Header("Sun Path")]
    [Range(0f, 90f)] public float sunElevation = 45f;
    [Range(0f, 360f)] public float azimuthOffsetDeg = 0f;

    [Header("Rain")]
    public RainOverlayController rainOverlay;
    public AudioSource rainLoop;
    [Range(0f, 1f)] public float rainVolume = 0.8f;

    [Header("Rain Timing")]
    public Vector2 dryDurationRange = new Vector2(30f, 90f);
    public Vector2 rainDurationRange = new Vector2(20f, 60f);
    public float rainBlendSeconds = 1.5f;

    [Header("Night Sky")]
    public Material nightSkybox;
    public ParticleSystem stars;

    [Header("Rain Sky")]
    public Material rainSkybox;

    [Header("Debug")]
    public Weather current = Weather.Dry;

    float _secondsPerDay;
    float _rainBlendT;
    bool _rainingTarget;
    float _baseSunIntensity;
    Material _daySkybox;

    [Header("Rain Particles")]
    public ParticleSystem rainParticles;
    public float rainRateMax = 1500f;

    ParticleSystem.EmissionModule _rainEm;


    void Awake()
    {
        _secondsPerDay = Mathf.Max(1f, dayLengthMinutes * 60f);
        if (sun) _baseSunIntensity = Mathf.Max(0.0001f, sun.intensity);
        if (rainLoop != null) rainLoop.volume = 0f;
        if (rainOverlay != null) rainOverlay.SetIntensity(0f);
        _daySkybox = RenderSettings.skybox;
        if (rainParticles){
            _rainEm = rainParticles.emission;
            _rainEm.enabled = false;
        }

    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(RainDirector());
    }

    void Update()
    {
        if (runClock)
        {
            time01 += Time.deltaTime / _secondsPerDay;
            if (time01 > 1f) time01 -= 1f;
        }

        UpdateSun();
        UpdateRainBlend(Time.deltaTime);
    }

    void UpdateSun()
    {
        if (!sun) return;

        float angle = time01 * 360f;
        Quaternion tilt = Quaternion.Euler(sunElevation, azimuthOffsetDeg, 0f);
        Quaternion spin = Quaternion.Euler(angle, 0f, 0f);
        sun.transform.rotation = tilt * spin * Quaternion.Euler(90f, 0f, 0f);

        float curveVal = Mathf.Max(0f, sunIntensityCurve.Evaluate(time01));
        sun.intensity = _baseSunIntensity * curveVal;
        if (sunColorGradient != null)
            sun.color = sunColorGradient.Evaluate(time01);

        Vector3 sunDir = -sun.transform.forward;
        bool isNight = sunDir.y <= 0f;

        Material baseSky = isNight && nightSkybox ? nightSkybox : _daySkybox;
        if (rainSkybox && _rainBlendT > 0.001f)
            RenderSettings.skybox = rainSkybox;
        else
            RenderSettings.skybox = baseSky;

        if (stars)
        {
            var em = stars.emission;
            em.enabled = isNight && (_rainBlendT <= 0.001f);
            if (em.enabled && !stars.isPlaying) stars.Play();
            if (!em.enabled && stars.isPlaying) stars.Stop();
        }
    }

    void UpdateRainBlend(float dt)
    {
        float target = _rainingTarget ? 1f : 0f;
        if (Mathf.Approximately(rainBlendSeconds, 0f))
            _rainBlendT = target;
        else
        {
            float speed = 1f / Mathf.Max(0.0001f, rainBlendSeconds);
            _rainBlendT = Mathf.MoveTowards(_rainBlendT, target, dt * speed);
        }

        if (rainOverlay != null)
            rainOverlay.SetIntensity(_rainBlendT);

        if (rainLoop != null)
        {
            rainLoop.volume = Mathf.Lerp(0f, rainVolume, _rainBlendT);
            if (_rainingTarget && !rainLoop.isPlaying) rainLoop.Play();
            if (!_rainingTarget && rainLoop.isPlaying && _rainBlendT <= 0f) rainLoop.Stop();
        }

        if (rainOverlay != null)
            rainOverlay.SetIntensity(_rainBlendT);

        if (rainParticles)
        {
            bool any = _rainBlendT > 0.001f;
            _rainEm.enabled = any;

            var rate = _rainEm.rateOverTime;
            rate.constant = Mathf.Lerp(0f, rainRateMax, _rainBlendT);
            _rainEm.rateOverTime = rate;

            if (any && !rainParticles.isPlaying) rainParticles.Play();
            if (!any && rainParticles.isPlaying) rainParticles.Stop();
        }

    }

    IEnumerator RainDirector()
    {
        while (true)
        {
            if (current == Weather.Dry)
            {
                float wait = Random.Range(dryDurationRange.x, dryDurationRange.y);
                yield return new WaitForSeconds(wait);
                SetRaining(true);
            }
            else
            {
                float wait = Random.Range(rainDurationRange.x, rainDurationRange.y);
                yield return new WaitForSeconds(wait);
                SetRaining(false);
            }
        }
    }

    public void SetTime01(float t)
    {
        time01 = Mathf.Repeat(t, 1f);
        UpdateSun();
    }

    public void SetRaining(bool on)
    {
        _rainingTarget = on;
        current = on ? Weather.Rain : Weather.Dry;
        if (Mathf.Approximately(rainBlendSeconds, 0f))
            SetRainImmediate(on);
    }

    public void SkipToMorning(float morningTime01 = 0.25f)
    {
        SetTime01(morningTime01);
    }

    void SetRainImmediate(bool on)
    {
        _rainBlendT = on ? 1f : 0f;

        if (rainOverlay != null)
            rainOverlay.SetIntensity(_rainBlendT);

        if (rainLoop != null)
        {
            rainLoop.volume = on ? rainVolume : 0f;
            if (on) rainLoop.Play();
            else rainLoop.Stop();
        }
    }

    void Reset()
    {
        sunIntensityCurve = new AnimationCurve(
            new Keyframe(0.00f, 0.00f),
            new Keyframe(0.20f, 0.60f),
            new Keyframe(0.50f, 1.00f),
            new Keyframe(0.80f, 0.60f),
            new Keyframe(1.00f, 0.00f)
        );
    }
}
