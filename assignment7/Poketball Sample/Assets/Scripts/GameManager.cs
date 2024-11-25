using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    UIManager MyUIManager;

    public GameObject BallPrefab;   // prefab of Ball

    // Constants for SetupBalls
    public static Vector3 StartPosition = new Vector3(0, 0, -6.35f);
    public static Quaternion StartRotation = Quaternion.Euler(0, 90, 90);
    const float BallRadius = 0.286f;
    const float RowSpacing = 0.02f;

    GameObject PlayerBall;
    GameObject CamObj;

    const float CamSpeed = 3f;

    const float MinPower = 15f;
    const float PowerCoef = 1f;

    void Awake()
    {
        // PlayerBall, CamObj, MyUIManager를 얻어온다.
        // ---------- TODO ---------- 
        PlayerBall = GameObject.Find("PlayerBall");
        CamObj = GameObject.Find("Main Camera");
        MyUIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        // -------------------- 
    }

    void Start()
    {
        SetupBalls();
    }

    // Update is called once per frame
    void Update()
    {
        // 좌클릭시 raycast하여 클릭 위치로 ShootBallTo 한다.
        // ---------- TODO ---------- 
        // Debug.DrawRay(CamObj.transform.position, CamObj.transform.forward * 100 , Color.red);
        // Debug.DrawRay(transform.position, Input.mousePosition, Color.blue);
        // Debug.DrawRay(CamObj.transform.position, Input.mousePosition);
        // Debug.Log(Input.mousePosition);
        // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // Vector3 point = Camera.main.ScreenToWorldPoint(
        //     new Vector3(
        //         Input.mousePosition.x, 
        //         Input.mousePosition.y, 
        //         Camera.main.transform.position.z)
        //     );
        // if(Input.GetMouseButtonDown(0)) {
        //     Debug.Log(point.ToString());
        //     ShootBallTo(point);
        // }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                // Debug.Log(hit.point);
                ShootBallTo(hit.point);
            }
        }
        // Camera.main.ScreenPointToRay(Input.mousePosition);


        // if (Input.GetKeyDown(KeyCode.Mouse0)) {
        //     RaycastHit hit;
        //     Vector3 directionVector = Input.mousePosition - CamObj.transform.position;
        //     if (Physics.Raycast(CamObj.transform.position, CamObj.transform.position + directionVector * 100, out hit)) {
        //         Vector3 loc = hit.point;
        //         Debug.Log(loc);
        //         ShootBallTo(loc);
        //     }
        // }
        // -------------------- 
    }

    void LateUpdate()
    {
        CamMove();
    }

    void SetupBalls()
    {
        // 15개의 공을 삼각형 형태로 배치한다.
        // 가장 앞쪽 공의 위치는 StartPosition이며, 공의 Rotation은 StartRotation이다.
        // 각 공은 RowSpacing만큼의 간격을 가진다.
        // 각 공의 이름은 {index}이며, 아래 함수로 index에 맞는 Material을 적용시킨다.
        // Obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/ball_1");
        // ---------- TODO ---------- 
        Vector3 BallPosition;
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j <= i; j++) {
                int ballNum = i*(i+1)/2+j+1;
                BallPosition = StartPosition + new Vector3(2*j-i, 0, -2*i) * (BallRadius + RowSpacing/2);
                GameObject Obj = Instantiate(BallPrefab, BallPosition, StartRotation);
                Obj.name = $"{ballNum}";
                Obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/ball_{ballNum}");
            }
        }
        // -------------------- 
    }
    void CamMove()
    {
        // CamObj는 PlayerBall을 CamSpeed의 속도로 따라간다.
        // ---------- TODO ---------- 
        if (PlayerBall) {
            Vector3 nextPosition = PlayerBall.transform.position;
            nextPosition.y = CamObj.transform.position.y;
            CamObj.transform.position = Vector3.Lerp(CamObj.transform.position, nextPosition, CamSpeed * Time.deltaTime);
        }
        // -------------------- 
    }

    float CalcPower(Vector3 displacement)
    {
        return MinPower + displacement.magnitude * PowerCoef;
    }

    void ShootBallTo(Vector3 targetPos)
    {
        // targetPos의 위치로 공을 발사한다.
        // 힘은 CalcPower 함수로 계산하고, y축 방향 힘은 0으로 한다.
        // ForceMode.Impulse를 사용한다.
        // ---------- TODO ---------- 
        if (PlayerBall) {
            Vector3 displacement = targetPos - PlayerBall.transform.position;
            displacement.y = 0;
            float force = CalcPower(displacement);
            PlayerBall.GetComponent<Rigidbody>().AddForce(force * displacement.normalized, ForceMode.Impulse);
        }
        // -------------------- 
    }
    
    // When ball falls
    public void Fall(string ballName)
    {
        // "{ballName} falls"을 1초간 띄운다.
        // ---------- TODO ---------- 
        if (ballName == "PlayerBall") {
            MyUIManager.DisplayText($"GameOver", 1f);
        } else {
            MyUIManager.DisplayText($"{ballName} falls", 1f);
        }
        // -------------------- 
    }
}
