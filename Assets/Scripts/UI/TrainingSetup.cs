using System;
using System.IO;
using System.Linq;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSetup : MonoBehaviour
{
    private static string InitialWorkingDir = Directory.GetCurrentDirectory();
    public GameObject TrainingSelectionScreen;
    public GameObject InstallProgressScreen;
    public GameObject InstallScreen;
    public GameObject TrainingSetupScreen;
    
    public TMP_Text InstallProgressText;
    public TMP_InputField InstallLocationField;
    public Button ContinueButton;
    public StringListView TrainingList;

    public Button LoadButton;
    public Button CloneButton;

    public PromptScreen Prompt;

    private string ProgressText = "";

    private static readonly string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                                                 "AgenceTrainingWorkspace";
    private void Awake()
    {
        var pyState = Python.CheckEnvironment();
        DontDestroyOnLoad(this);

        switch (pyState)
        {
            case EnvironmentState.NoPython:
                throw new Exception("No Python installed");
            case EnvironmentState.NoVirtualEnv:
                InstallScreen.SetActive(true);
                break;
            case EnvironmentState.AllGood:
                ActivateTrainingScreen();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        InstallLocationField.text = Python.WorkspacePath ?? DefaultPath;

    }

    public void Event_Browse()
    {
        var currentPath = Directory.Exists(InstallLocationField.text) ? InstallLocationField.text : DefaultPath;
        var installPath = StandaloneFileBrowser.OpenFolderPanel("ChooseWorkspaceLocation", currentPath, false);
        if (installPath.Length > 0) InstallLocationField.text = installPath[0];


    }

    public void Event_Install()
    {
        InstallScreen.SetActive(false);
        InstallProgressScreen.SetActive(true);
        ContinueButton.interactable = false;

        Python.InstallWorkspace(InstallLocationField.text);


        Python.InstallEnv(progressStr => ProgressText += "\n"+progressStr, OnInstallExit);
    }


    

    public void Event_Continue()
    {
        InstallProgressScreen.SetActive(false);
        ActivateTrainingScreen();
        

    }

    private void ActivateTrainingScreen()
    {
        TrainingSelectionScreen.SetActive(true);

        var directories = Directory.GetDirectories(Python.TrainingsFolder).Select(Path.GetFileName);

        TrainingList.UpdateButtons(directories.ToList());

    }

    public void Event_LoadClicked()
    {
        var trainingDir = Path.Combine(Python.TrainingsFolder, TrainingList.SelectedString);

        Directory.SetCurrentDirectory(trainingDir);
        Debug.Log(Directory.GetCurrentDirectory());
        TrainingSelectionScreen.SetActive(false);
        TrainingSetupScreen.SetActive(true);
    }


    public void Event_CloneClicked()
    {
        Prompt.Show("Name the New Training", newDir => Clone(TrainingList.SelectedString, newDir));
    }

    private void Clone(string originalDir, string newDir)
    {
        originalDir = Path.Combine(Python.TrainingsFolder, originalDir);
        newDir = Path.Combine(Python.TrainingsFolder, newDir);
        FileUtils.DirectoryCopy(originalDir, newDir, true);
        ActivateTrainingScreen();
    }

    public void Event_New()
    {
        Prompt.Show("Name the New Training", CreateTraining);

    }

    private void CreateTraining(string newDir)
    {
        Directory.CreateDirectory(Path.Combine(Python.TrainingsFolder, newDir));
        ActivateTrainingScreen();
    }

    private void Update()
    {

        InstallProgressText.text = ProgressText;
        var validSelection = TrainingList.SelectedIndex >= 0 && TrainingList.SelectedIndex < TrainingList.Count;
        LoadButton.interactable = validSelection;
        CloneButton.interactable = validSelection;

    }


    private void OnInstallExit(int exitCode)
    {
        var text = ContinueButton.GetComponentInChildren<TMP_Text>();

        text.text = exitCode == 0 ? "Installation Successful!" : "Installation Failed. (Go Back?)";
        ContinueButton.interactable = true;
    }

    public void OnApplicationQuit()
    {
        Directory.SetCurrentDirectory(InitialWorkingDir);
    }

}