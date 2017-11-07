using UnityEngine;

public class ParticleMoveTarget : MonoBehaviour {

    public Transform sourceTransform;
    public Transform targetTransform;

    public Vector3 rotSpeed = new Vector3(0, 0, 0);
    public Vector3 moveSpeed = new Vector3(0, 0, 0);
    private Vector3 destiny = new Vector3(0, 0, 0);
    public float time = 0f;
    public bool pause = false;
    public float delay = 0f;
    public bool accelerateRotate = false;
    public float accelerateRotateTime = 0f;

	public bool loop = false;

    private float startTime;
    private Vector3 birthPosition;
    private bool useSpeed = false;
    private bool useTime = false;
    private Transform myTransform;
    private float timePast = 0f;
    private float timePastInit = 0f;

    // Use this for initialization
    void OnEnable()
    {
        myTransform = transform;
       
        destiny = targetTransform.position;

        timePast = timePastInit;

        startTime = Time.time;
        if (sourceTransform == null)
        {
            birthPosition = myTransform.position;
        }
        else
        {
            birthPosition = sourceTransform.position;
        }
        if (time > 0f)
            useTime = true;
        else
            useSpeed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime + delay > Time.time) return;
        if (pause) return;
        Vector3 rotateSpeed = rotSpeed;
        if (accelerateRotate && accelerateRotateTime > 0f && startTime + delay + accelerateRotateTime > Time.time)
        {
            rotateSpeed = rotSpeed * ((Time.time - startTime - delay) / accelerateRotateTime);
        }
        myTransform.Rotate(rotateSpeed * Time.deltaTime);
        if (useSpeed)
        {
            myTransform.Translate(moveSpeed * Time.deltaTime);
        }
        else if (useTime)
        {
            if (timePast <= time)
            {
                myTransform.position = Vector3.Lerp(birthPosition, destiny, timePast / time);
                timePast += Time.deltaTime;
            }
			else if (loop)
			{
				timePast = 0f;
				myTransform.position = birthPosition;
			}
        }
    }
}
