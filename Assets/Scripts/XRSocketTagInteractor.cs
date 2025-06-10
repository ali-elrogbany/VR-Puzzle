using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRSocketTagInteractor : XRSocketInteractor
{
    [Tooltip("Only objects with this tag can be snapped into this socket.")]
    public string targetTag;

    public ObjectColor targetColor;

    [Header("Sound Effects")]
    public AudioClip satisfiedSFX;
    public AudioClip unsatisfiedSFX;

    private AudioSource audioSource;
    private bool isSatisfied = false;
    private XRBaseInteractable lockedInteractable = null;

    void Awake()
    {
        base.Awake();
        // Get or add an AudioSource to this GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

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

            PlaySFX(satisfiedSFX);
            StartCoroutine(LockObjectAfterSnap(go, lockedInteractable));
        }
        else
        {
            Debug.Log("UnSatisfied");
            PlaySFX(unsatisfiedSFX);
        }
    }

    private IEnumerator LockObjectAfterSnap(GameObject go, XRBaseInteractable interactable)
    {
        yield return new WaitForSeconds(1f);

        XRGrabInteractable grabInteractable = go.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (interactionManager != null && interactable != null)
        {
            interactionManager.SelectEnter(this, interactable);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
