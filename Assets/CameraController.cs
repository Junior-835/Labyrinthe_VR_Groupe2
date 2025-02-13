using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;    // Vitesse de déplacement
    public float lookSpeed = 2f;    // Vitesse de rotation avec la souris
    public float rotationSpeed = 10f; // Vitesse de rotation automatique
    public float gravity = 9.81f;   // Gravité
    public float rotationDamping = 0.01f; // Facteur de réduction de la vitesse de déplacement lors de la rotation
    public float rotationSmoothness = 0.1f; // Lissage de la rotation lorsque l'on ne se déplace pas

    private CharacterController controller;
    private float rotationX = 0f;   // Stocke la rotation verticale
    private Vector3 moveDirection;  // Stocke la direction du déplacement
    private Quaternion targetRotation; // Rotation cible pour un lissage

    void Start()
    {
        controller = GetComponent<CharacterController>(); // Récupérer le CharacterController

        // Optionnel : cacher le curseur au démarrage
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Mets true si tu veux voir le curseur
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) // Appuie sur "Shift" pour courir
        {
            moveSpeed = 10f; // Accélérer
        }
        else
        {
            moveSpeed = 5f; // Vitesse normale
        }

        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D ou ←/→
        float moveZ = Input.GetAxis("Vertical");   // W/S ou ↑/↓

        float adjustedMoveSpeed = moveSpeed;

        // Si aucune touche n'est enfoncée mais que le joueur appuie sur la touche gauche ou droite du clavier, réduire la vitesse
        if (moveX == 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))  // Appuie sur "Flèche gauche" ou "Flèche droite"
            {
                adjustedMoveSpeed *= 0.08f; // Réduire la vitesse
            }
        }
        else
        {
            adjustedMoveSpeed *= rotationDamping; // Réduire la vitesse lorsque la caméra tourne
        }

        // Convertir l'entrée en direction 3D
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        moveDirection = move * adjustedMoveSpeed;

        // Appliquer la gravité
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Déplacer avec collisions gérées par le CharacterController
        controller.Move(moveDirection * Time.deltaTime);

        // Nouvelle rotation automatique dans la direction du mouvement
        if (moveX != 0 || moveZ > 0)
        {
            targetRotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
        }
        else
        {
            targetRotation = Quaternion.identity; // Evite que la caméra continue de tourner sans mouvement
        }
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Rotation avec le clic droit
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Empêcher de regarder trop haut/bas

            // Lissage de la rotation de la souris
            transform.localRotation = Quaternion.Euler(rotationX, transform.localRotation.eulerAngles.y + mouseX, 0);
        }
        else // Si la souris n'est pas activée, lisser la rotation automatique
        {
            if (targetRotation != Quaternion.identity)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
            }
        }
    }
}
