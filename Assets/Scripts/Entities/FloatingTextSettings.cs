using System;
using TMPro;
using UnityEngine;
namespace TwilightAndBlight
{
    [CreateAssetMenu(menuName = "Floating Text Settings")]
    public class FloatingTextSettings: ScriptableObject
    {
        public Vector2 InitialPosition { get; set; }
        [SerializeField] private Vector2 initialPositionXRange;
        [SerializeField] private Vector2 initialPositionYRange;

        public Vector2 InitialVelocity { get; set; }
        [SerializeField] private Vector2 initialVelocityXRange;
        [SerializeField] private Vector2 initialVelocityYRange;

        public Vector2 Acceleration { get; set; }
        [SerializeField] private Vector2 accelerationXRange;
        [SerializeField] private Vector2 accelerationYRange;

        public float InitialRotation { get; set; }
        [SerializeField] private Vector2 initialRotationRange;

        public float InitialAngularVelocity {  get; set; }
        [SerializeField] private Vector2 initialAngularVelocityRange;  
        

        public float AngularAcceleration { get; set; }
        [SerializeField] private Vector2 angularAccelerationRange;


        public float InitialTextSize { get; set; }
        [SerializeField] private Vector2 initialTextSizeRange;
        [SerializeField] private AnimationCurve sizeOverLifetime = default;
        [SerializeField] private Gradient colorOverLifetime = default;
        [SerializeField] private AnimationCurve opacityOverLifetime = default;
        public AnimationCurve SizeOverLifetime { get { return sizeOverLifetime; } set { sizeOverLifetime = value; } }
        public Gradient ColorOverLifetime { get { return colorOverLifetime; } set { colorOverLifetime = value; } }
        public AnimationCurve OpacityOverLifetime { get { return opacityOverLifetime; } set { opacityOverLifetime = value; } }

        public float TextDuration { get; set; }
        [SerializeField] private Vector2 textDurationRange;




        public void GenerateSettingInstance()
        {
            InitialPosition = new Vector2(GetRandomValueInRange(initialPositionXRange), GetRandomValueInRange(initialPositionYRange));
            InitialVelocity = new Vector2(GetRandomValueInRange(initialVelocityXRange), GetRandomValueInRange(initialVelocityYRange));
            Acceleration = new Vector2(GetRandomValueInRange(accelerationXRange), GetRandomValueInRange(accelerationYRange));
            InitialRotation = GetRandomValueInRange(initialRotationRange);
            InitialAngularVelocity = GetRandomValueInRange(initialAngularVelocityRange);
            AngularAcceleration = GetRandomValueInRange(angularAccelerationRange);
            TextDuration = GetRandomValueInRange(textDurationRange);
            InitialTextSize = GetRandomValueInRange(initialTextSizeRange);
        }
        
        


        private float GetRandomValueInRange(Vector2 range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }

    }
}