using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class Robot : Agent
{
    [Header("速度"), Range(1, 50)]
    public float speed=10;

    /// <summary>
    /// 機器人鋼體
    /// </summary>
    private Rigidbody rigRobot;
    /// <summary>
    /// 足球鋼體
    /// </summary>
    private Rigidbody rigBall;

    private void Start()
    {
        rigRobot = GetComponent<Rigidbody>();
        rigBall = GameObject.Find("足球").GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 事件開始時:重新設定機器人與足球的位置
    /// </summary>
    public override void OnEpisodeBegin()
    {
        //重設鋼體嘉速度與角色加速度
        rigRobot.velocity = Vector3.zero; 
        rigRobot.angularVelocity = Vector3.zero; 
        rigBall.velocity = Vector3.zero; 
        rigBall.angularVelocity = Vector3.zero;

        //隨機機器人位置 x左右 z前後
        Vector3 posRobot  = new Vector3(Random.Range(-1f, 1f), 0.1f, Random.Range(-1f, 0f)); 
        transform.position = posRobot;

        //隨機足球位置
        Vector3 posBall = new Vector3(Random.Range(-0.5f, 0.5f), 0.1f, Random.Range(1f, 1.5f));
        transform.position = posBall ;

        //足球尚未進入球門
        Ball.complete = false;
    }

    /// <summary>
    /// 收集觀測資料
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // 加入觀測資料:機器人 足球座標 機器人加速度 x z 
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rigBall.position);
        sensor.AddObservation(rigRobot.velocity.x);
        sensor.AddObservation(rigRobot.velocity.z);
    }

    /// <summary>
    /// 動作:控制機器人與回饋
    /// </summary>
    public override void OnActionReceived(float[] vectorAction)
    {
        //使用參數控制機器人
        Vector3 control = Vector3.zero;
        control.x = vectorAction[0];
        control.z = vectorAction[1];
        rigRobot.AddForce(control * speed);

        //球進入球門 成功:加1分並結束
        if (Ball.complete)
        {
            SetReward(1);
            EndEpisode();
        }

        //機器人或足球掉到地板下方 失敗:扣1分並結束
        if (transform.position.y < 0 || rigBall.position.y < 0)
        {
            SetReward(-1);
            EndEpisode();
        }

    }

    /// <summary>
    /// 探索:讓開發者測試環境
    /// </summary>
    /// <returns></returns>

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

}
