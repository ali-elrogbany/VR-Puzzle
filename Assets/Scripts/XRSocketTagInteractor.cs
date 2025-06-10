using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRSocketTagInteractor : XRSocketInteractor
{
    [Tooltip("Only objects with this tag can be snapped into this socket.")]
    public string targetTag;

    public ObjectColor targetColor;

    private bool isSatisfied = false;
    private XRBaseInteractable lockedInteractable = null;

    public bool GetIsSatisfied()
    {
        return isSatisfied;
    }

    public override bool CanHover(XRBaseInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.CompareTag(targetTag);
    }

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        // Always allow selection — we’ll disable interaction manually later
        return base.CanSelect(interactable) && interactable.CompareTag(targetTag);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (isSatisfied)
            return;

        base.OnSelectEntered(args);

        GameObject go = args.interactableObject.transform.gameObject;
        BoxController boxController = go.GetComponent<BoxController>();

        if (boxController != null && boxController.GetObjectColor() == targetColor)
        {
            isSatisfied = true;
            lockedInteractable = args.interactableObject as XRBaseInteractable;
            Debug.Log("Satisfied");

            StartCoroutine(LockObjectAfterSnap(go, lockedInteractable));
        }
        else
        {
            Debug.Log("UnSatisfied");
        }
    }

    private IEnumerator LockObjectAfterSnap(GameObject go, XRBaseInteractable interactable)
    {
        // Wait for one frame to allow snapping to complete
        yield return new WaitForSeconds(1f);

        // Disable grab interactable
        XRGrabInteractable grabInteractable = go.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        // Set Rigidbody to kinematic
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Ensure it stays selected in the socket
        if (interactionManager != null && interactable != null)
        {
            interactionManager.SelectEnter(this, interactable);
        }
    }
}
