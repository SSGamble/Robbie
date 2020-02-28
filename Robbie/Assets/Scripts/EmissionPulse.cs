using UnityEngine;

/// <summary>
/// 控制点光源，让背景泛光，产生若隐若现的效果
/// </summary>
public class EmissionPulse : MonoBehaviour {
    public float maxIntensity = 15f;    //The max emissive intensity
    public float damping = 2f;          //The damping to control the pulse speed

    Material material;                  //The material being controlled
    int emissionColorProperty;          //The ID of the emission property


    void Start() {
        // 获取Renderer组件的引用，这样我们就可以存储它的材质
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;

        //Convert the property ID string to an integer. This is much more efficient
        //than using strings to control material properties
        emissionColorProperty = Shader.PropertyToID("_EmissionColor");
    }

    void Update() {
        //Calculate the emission value based on Time and intensity
        float emission = Mathf.PingPong(Time.time * damping, maxIntensity);

        //Convert this to a color value
        Color finalColor = Color.white * emission;

        //Apply the color to the material
        material.SetColor(emissionColorProperty, finalColor);
    }
}
