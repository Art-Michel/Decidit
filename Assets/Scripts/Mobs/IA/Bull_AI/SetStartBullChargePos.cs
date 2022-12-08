using UnityEngine;

[ExecuteInEditMode]
public class SetStartBullChargePos : MonoBehaviour
{
    [SerializeField] enum State { Forward, ForwardRight, ForwardLeft, Right, Left};
    [Header("Direction")]
    [SerializeField] State state;

    [SerializeField] float pos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Forward)
        {
            transform.localPosition = new Vector3(0, transform.localPosition.y, pos);
        }
        else if (state == State.ForwardRight)
        {
            transform.localPosition = new Vector3(pos, transform.localPosition.y, pos);
        }
        else if (state == State.ForwardLeft)
        {
            transform.localPosition = new Vector3(-pos, transform.localPosition.y, pos);
        }
        else if (state == State.Right)
        {
            transform.localPosition = new Vector3(pos, transform.localPosition.y, 0);
        }
        else if (state == State.Left)
        {
            transform.localPosition = new Vector3(-pos, transform.localPosition.y, 0);
        }
    }
}