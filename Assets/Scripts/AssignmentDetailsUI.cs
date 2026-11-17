using UnityEngine;
using TMPro;

public class AssignmentDetailsUI : MonoBehaviour
{
    [Header("Data Source")]
    public AssignmentDetails details;

    [Header("UI References")]
    public TMP_Text studentNameText;
    public TMP_Text assignmentNameText;
    public TMP_Text mechanic1Text;
    public TMP_Text mechanic2Text;
    public TMP_Text mechanic3Text;

    void Start()
    {
        if (details == null)
        {
            Debug.LogError("AssignmentDetails not assigned!");
            return;
        }

        studentNameText.text = details.studentName;
        assignmentNameText.text = details.assignmentName;
        mechanic1Text.text = details.mechanic1;
        mechanic2Text.text = details.mechanic2;
        mechanic3Text.text = details.mechanic3;
    }
}
