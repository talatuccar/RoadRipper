using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;



public class PlayerManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isSwiping = false;

    [SerializeField] private float moveDistance = 2.5f;
    [SerializeField] private float laneTransitionTime = 0.2f;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float jumpSwipeThreshold = 100f;

    private bool isGrounded = true;
    private Animator animator;
    private CharacterController controller;

    private enum Lanes { Left = -1, Middle = 0, Right = 1 }
    private Lanes currentLane = Lanes.Middle;

    private Vector3 velocity;
    private float gravity = -9.81f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerInput.actions["TouchPress"].started += ctx => StartTouch(ctx);
        playerInput.actions["TouchPress"].canceled += ctx => EndTouch(ctx);
    }

    private void OnDisable()
    {
        playerInput.actions["TouchPress"].started -= ctx => StartTouch(ctx);
        playerInput.actions["TouchPress"].canceled -= ctx => EndTouch(ctx);
    }

    private void Update()
    {
        // S�rekli ileri hareket
        Vector3 move = transform.forward * forwardSpeed * Time.deltaTime;
        controller.Move(move);

        // Yer�ekimi
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Hafif bir yere temas etkisi
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        isSwiping = true;
        startTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();
        
    }
   
    private void EndTouch(InputAction.CallbackContext context)
    {
        if (isSwiping)
        {
            isSwiping = false;
            currentTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();

            Vector2 swipeDirection = currentTouchPosition - startTouchPosition;

       
            if (Mathf.Abs(swipeDirection.x) > swipeThreshold)
            {
                if (swipeDirection.x > 0)
                {
                    MoveRight();
                }
                else
                {
                    MoveLeft();
                }
            }
            if (Mathf.Abs(swipeDirection.y) > jumpSwipeThreshold && Mathf.Abs(swipeDirection.x) <= jumpSwipeThreshold)
            {
                Jump(); // Hemen z�pla
            }
        }
    }

    private void MoveRight()
    {
        if (currentLane == Lanes.Right)
        {
            return; // Zaten sa� �eritte, ��k
        }

        currentLane++;
        MoveToLane(currentLane);
    }

    private void MoveLeft()
    {
        if (currentLane == Lanes.Left)
        {
            return; // Zaten sol �eritte, ��k
        }

        currentLane--;
        MoveToLane(currentLane);
    }




    private void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
            animator.SetTrigger("Jump");
            isGrounded = false;
            Debug.Log("Z�pland�!");
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void MoveToLane(Lanes lane)
    {
        float targetX = (int)lane * moveDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        StartCoroutine(SmoothLaneTransition(targetPosition));
    }

    private IEnumerator SmoothLaneTransition(Vector3 targetPosition)
    {
        Vector3 startingPos = transform.position;
        float elapsed = 0f;

        while (elapsed < laneTransitionTime)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsed / laneTransitionTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}

































public class SwipeMovement : MonoBehaviour
{


    private PlayerInput playerInput;
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isSwiping = false;

    [SerializeField] private float moveDistance = 2.5f;        // Karakterin sa�a ve sola ge�ece�i mesafe
    [SerializeField] private float laneTransitionTime = 0.2f;  // Ge�i� s�resi
    [SerializeField] private float forwardSpeed = 5f;          // Karakterin s�rekli ileri h�z�
    [SerializeField] private float swipeThreshold = 50f;       // Kayd�rma e�i�i (piksel cinsinden)
    [SerializeField] private float jumpForce = 10f;            // Z�plama g�c�
    [SerializeField] private float jumpSwipeThreshold = 100f;  // Z�plama i�in kayd�rma e�i�i
    private bool isGrounded = true;                            // Z�plama kontrol�

    private Animator animator;
    //private Rigidbody rb;

    // Enum ile �eritler
    private enum Lanes { Left = -1, Middle = 0, Right = 1 }
    private Lanes currentLane = Lanes.Middle;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerInput.actions["TouchPress"].started += ctx => StartTouch(ctx);
        playerInput.actions["TouchPress"].canceled += ctx => EndTouch(ctx);
    }

    private void OnDisable()
    {
        playerInput.actions["TouchPress"].started -= ctx => StartTouch(ctx);
        playerInput.actions["TouchPress"].canceled -= ctx => EndTouch(ctx);
    }

    private void Update()
    {
        //// S�rekli ileri hareket
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        //rb.velocity = Vector3.forward * forwardSpeed * Time.deltaTime;
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        isSwiping = true;
        startTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        if (isSwiping)
        {
            isSwiping = false;
            currentTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();

            Vector2 swipeDirection = currentTouchPosition - startTouchPosition;

            // Z�plama i�lemi i�in yukar� kayd�rma kontrol�
            if (swipeDirection.y > jumpSwipeThreshold && isGrounded)
            {
                Jump();
            }
            // Sa�a veya sola kayd�rma kontrol�
            else if (Mathf.Abs(swipeDirection.x) > swipeThreshold)
            {
                if (swipeDirection.x > 0)
                {
                    MoveRight();
                }
                else
                {
                    MoveLeft();
                }
            }
        }
    }

    private void MoveRight()
    {
        if (currentLane != Lanes.Right) // En sa�da de�ilse
        {
            currentLane++;
            MoveToLane(currentLane);
            Debug.Log("Sa�a ge�ildi.");
        }
    }

    private void MoveLeft()
    {
        if (currentLane != Lanes.Left) // En solda de�ilse
        {
            currentLane--;
            MoveToLane(currentLane);
            Debug.Log("Sola ge�ildi.");
        }
    }

    private void Jump()
    {
        // Z�plama animasyonu ve fizik
        if (isGrounded)
        {
            //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            isGrounded = false;
            Debug.Log("Z�pland�!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Karakter yere d��t���nde tekrar z�playabilir hale gelir
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            print(isGrounded);
        }
    }
    

    private void MoveToLane(Lanes lane)
    {
        // �erit ge�i�i i�in animasyon/fizik
        float targetX = (int)lane * moveDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        StartCoroutine(SmoothLaneTransition(targetPosition));
    }

    private IEnumerator SmoothLaneTransition(Vector3 targetPosition)
    {
        Vector3 startingPos = transform.position;
        float elapsed = 0f;

        while (elapsed < laneTransitionTime)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsed / laneTransitionTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}


















































































































































































































//using DG.Tweening;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class SwipeMovement : MonoBehaviour
//{

//    //DOTWEEN YONTEM�YLE
//    private PlayerInput playerInput;
//    private Vector2 startTouchPosition;
//    private Vector2 currentTouchPosition;
//    private bool isSwiping = false;

//    [SerializeField] private float moveDistance = 2.5f;
//    [SerializeField] private float laneTransitionTime = 0.2f;
//    private Vector3 targetPosition;

//    private enum Lanes { Left = -1, Middle = 0, Right = 1 }
//    private Lanes currentLane = Lanes.Middle;

//    [SerializeField] private float forwardSpeed = 5f;
//    [SerializeField] private float swipeThreshold = 50f;

//    private void Awake()
//    {
//        playerInput = GetComponent<PlayerInput>();
//    }

//    private void OnEnable()
//    {
//        playerInput.actions["TouchPress"].started += ctx => StartTouch(ctx);
//        playerInput.actions["TouchPress"].canceled += ctx => EndTouch(ctx);
//    }

//    private void OnDisable()
//    {
//        playerInput.actions["TouchPress"].started -= ctx => StartTouch(ctx);
//        playerInput.actions["TouchPress"].canceled -= ctx => EndTouch(ctx);
//    }

//    private void Update()
//    {
//        // S�rekli ileri hareket
//        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
//    }

//    private void StartTouch(InputAction.CallbackContext context)
//    {
//        isSwiping = true;
//        startTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();
//    }

//    private void EndTouch(InputAction.CallbackContext context)
//    {
//        if (isSwiping)
//        {
//            isSwiping = false;
//            currentTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();

//            Vector2 swipeDirection = currentTouchPosition - startTouchPosition;

//            if (Mathf.Abs(swipeDirection.x) > swipeThreshold)
//            {
//                if (swipeDirection.x > 0)
//                {
//                    MoveRight();
//                }
//                else
//                {
//                    MoveLeft();
//                }
//            }
//        }
//    }

//    private void MoveRight()
//    {
//        if (currentLane == Lanes.Right)
//        {
//            return; // Zaten sa� �eritte, ��k
//        }

//        currentLane++;
//        MoveToLane(currentLane);
//    }

//    private void MoveLeft()
//    {
//        if (currentLane == Lanes.Left)
//        {
//            return; // Zaten sol �eritte, ��k
//        }

//        currentLane--;
//        MoveToLane(currentLane);
//    }


//    private void MoveToLane(Lanes lane)
//    {
//        float targetX = (int)lane * moveDistance;
//        targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

//        // DoTween ile ge�i�
//        transform.DOMove(targetPosition, laneTransitionTime).SetEase(Ease.OutQuad);
//    }
//}















































































































































// vector3 LERP YONTEM�YLE

//using UnityEngine;
//using UnityEngine.InputSystem;
//using System.Collections;

//public class SwipeMovement : MonoBehaviour
//{
//    private PlayerInput playerInput;
//    private Vector2 startTouchPosition;
//    private Vector2 currentTouchPosition;
//    private bool isSwiping = false;

//    [SerializeField] private float moveDistance = 2.5f;          // Karakterin sa�a ve sola ge�ece�i mesafe
//    [SerializeField] private float laneTransitionTime = 0.2f;    // Ge�i� s�resi
//    private Vector3 targetPosition;

//    bool isMiddle = true;
//    bool canSwipeRight = true;
//    bool canSwipeLeft = true;
//    //private int currentLane = 1;                // 0: Sol, 1: Orta, 2: Sa�
//    //private int maxLanes = 3;                    // Toplam lane say�s�


//    enum Lanes
//    {

//        left = -1,
//        middle = 0,
//        right = 1,

//    }
//    private Lanes currentLane;


//    [SerializeField] private float forwardSpeed = 5f;             // Karakterin s�rekli ileri h�z�
//    [SerializeField] private float swipeThreshold = 50f;          // Kayd�rma e�i�i (piksel cinsinden)

//    private void Awake()
//    {
//        currentLane = Lanes.middle;
//        playerInput = GetComponent<PlayerInput>();
//    }

//    private void OnEnable()
//    {
//        playerInput.actions["TouchPress"].started += ctx => StartTouch(ctx);
//        playerInput.actions["TouchPress"].canceled += ctx => EndTouch(ctx);
//    }

//    private void OnDisable()
//    {
//        playerInput.actions["TouchPress"].started -= ctx => StartTouch(ctx);
//        playerInput.actions["TouchPress"].canceled -= ctx => EndTouch(ctx);
//    }

//    private void Update()
//    {
//        // S�rekli ileri hareket
//        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
//    }

//    private void StartTouch(InputAction.CallbackContext context)
//    {
//        isSwiping = true;
//        startTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();
//    }

//    private void EndTouch(InputAction.CallbackContext context)
//    {
//        if (isSwiping)
//        {
//            isSwiping = false;
//            currentTouchPosition = playerInput.actions["TouchPosition"].ReadValue<Vector2>();

//            Vector2 swipeDirection = currentTouchPosition - startTouchPosition;

//            if (Mathf.Abs(swipeDirection.x) > swipeThreshold)
//            {
//                if (swipeDirection.x > 0)
//                {
//                    MoveRight();
//                }
//                else
//                {
//                    MoveLeft();
//                }
//            }
//        }
//    }



//    private void MoveRight()
//    {
//        if (isMiddle)
//        {
//            currentLane = Lanes.right;
//            isMiddle = false;
//            canSwipeRight = false;
//            canSwipeLeft = true;
//        }
//        else
//        {
//            if (canSwipeRight == false)
//                return;

//            currentLane = Lanes.middle;
//            isMiddle = true;

//        }


//        StopAllCoroutines();
//        StartCoroutine(MoveToLane(currentLane));
//        Debug.Log("Sa�a ge�ildi.");

//    }

//    private void MoveLeft()
//    {
//        if (isMiddle)
//        {
//            currentLane = Lanes.left;
//            isMiddle = false;
//            canSwipeLeft = false;
//            canSwipeRight = true;

//        }
//        else
//        {
//            if (canSwipeLeft == false)
//                return;
//            currentLane = Lanes.middle;
//            isMiddle = true;

//        }


//        StopAllCoroutines();
//        StartCoroutine(MoveToLane(currentLane));
//        Debug.Log("Sola ge�ildi.");

//    }

//    private IEnumerator MoveToLane(Lanes lane)
//    {
//        float elapsed = 0f;
//        Vector3 startingPos = transform.position;
//        float targetX = (int)lane * moveDistance; // Orta lane 0, sa� lane +2.5, sol lane -2.5

//        targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

//        while (elapsed < laneTransitionTime)
//        {
//            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsed / laneTransitionTime);
//            elapsed += Time.deltaTime;
//            yield return null;
//        }

//        transform.position = targetPosition;
//    }
//}
