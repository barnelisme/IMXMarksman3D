using UnityEngine;
using TMPro;

public class SceneInfoManager : MonoBehaviour
{
    [Header("Holders")]
    public TextMeshProUGUI scene_name_label;
    public TMP_Dropdown scene_info_dropdown;

    private string simulator_scene_name = "";
    private string build_scene_name = "";
    private string scene_info = "";

    void Start()
    {
        LoadInfo();
    }

    private void LoadInfo()
    {
        //Scenarios :  * Bullseye Challenge * Man Threat 2 * Man Threat 8 * Hostage Situation * IPEC Man 4
        //* IPEC Man 10 * Diverse Bullseye * Pole Alignment * Seqnum * Seqnum Addition * ROYGBIV * Indoor range 1 Lane
        //* Indoor range 2 Lane * Indoor range 3 Lane * Distance Simulator * Falling Plates * Shifting IPEC Plates
        //* Basic IPEC Board * Rising Target * Man Threat 2 * Man Threat 8 * Hostage Situation * IPEC Man 4 * IPEC Man 10
        //* Diverse Bullseye * Dice Shoot * Shell Game * Dueling Tree * Ascending Balloons * Racetrack Target * Hidden Shape
        //* Clay Pigeon * Rising Shape


        //Check scene name
        simulator_scene_name = DropDown.ompSceneName;
        build_scene_name = DropDown.softwareSceneName;
        //print("Scene: " + simulator_scene_name);

        //Load Info
        if (simulator_scene_name.ToLower().Contains("bullseye challenge"))
        {
            scene_info = "This scenario contain 5 bullseye targets per lane. Each target has ten points for the center, 5 points" +
                "for the middle line, and 2 points for the outer line. The objective is to hit the center point.";
        }
        else if(simulator_scene_name.ToLower().Contains("rising target"))
        {
            scene_info = "This scenario contains plates as targets. On startup you will see a plate at the bottom of your lane, after" +
                "hitting the plate, it will slitely move up and become smaller in size.";
        }
        else if (simulator_scene_name.ToLower().Contains("balloon"))
        {
            scene_info = "This scenario contains balloons as targets, and it is colour based. Balloons will appear from the " +
                "bottom of the screen in different colours, the objective is follow the colour indicatior at the botton left and right of" +
                " your training view. Note: You should only shoot the correct coloured balloons, shooting the wrong color " +
                "will reduce points. " /*+
                "NOTE: Only shoot balloons within the two red horizontal barrier lines."*/ ; 
        }
        else if (simulator_scene_name.ToLower().Contains("man threat") || simulator_scene_name.ToLower().Contains("ipec Man")) //IPEC Man
        {
            if (build_scene_name.ToLower().Contains("static"))
            {
                scene_info = "This scenario contains an IPEC. When you hit the shaded areas you will hear a sound and the area will" +
                    " change colour to blue. Training is based on body shots and head shots.";
            }
            else
            {
                scene_info = "This scenario contains IPEC Boards. When you hit the board it moves to another position. When you hit " +
                    "the shaded areas you will hear a hit sound, and the area will change colour to blue. Training is based on body shots " +
                    "and head shots.";
            }
        }
        else if (simulator_scene_name.ToLower().Contains("hostage situation"))
        {
            if (build_scene_name.ToLower().Contains("static"))
            {
                scene_info = "This scenario contains an IPEC. You have a good guy and a bad guy, the objective is to shoot the bad guy." +
                    " When you hit the shaded areas you will hear a sound and the area will change colour to blue. Training is based" +
                    " on body shots and head shots.";
            }
            else
            {
                scene_info = "This scenario contains IPEC Boards. You have a good guy and a bad guy, the objective is to shoot the bad guy." +
                    " When you hit the board it moves to another position. When you hit the shaded areas you will hear a hit sound, and " +
                    "the area will change colour to blue. Training is based on body shots and head shots.";
            }
        }
        else if (simulator_scene_name.ToLower().Contains("roygbiv"))
        {
            scene_info = "This scenario contains plates as targets, and the training is based of colour and memory. On startup, the plates " +
                "display a certain colour, and you have to memorise the colour of each plate. After a few seconds the plates all " +
                "go black, and you have to shoot the correct colour based on the indicator.";
        }
        else if (simulator_scene_name.ToLower().Contains("seqnum")) //Seqnum * Seqnum Addition
        {

            if(simulator_scene_name.ToLower().Contains("addition"))
            {
                scene_info = "This scenario contains plates as targets, and the training is based of numbers, memory, and summing. On startup, the plates " +
                "display a certain number, and you have to memorise the number of each plate. After a few seconds the plates all " +
                "go blank, and you will see a target sum on your right. you have to hit the correct numbers untill you reach the final answer.";
            }
            else
            {
                scene_info = "This scenario contains plates as targets, and the training is based of numbers and memory. On startup, the plates " +
                "display a certain number, and you have to memorise the number of each plate. After a few seconds the plates all " +
                "go blank, and you have to shoot the correct number based on the indicator.";
            }
        }
        else if (simulator_scene_name.ToLower().Contains("racetrack target"))
        {
            scene_info = "This scenario contains plates as targets, and cars as progress reports. The plates are seperated with coloured areas," +
                " where each coloured area has its own dedicated car. The objective is to hit the moving plate and the car will move. The car leading" +
                " the run is the winner. Note: Do not shoot when cars are blinking.";
        }
        else if (simulator_scene_name.ToLower().Contains("hidden shape"))
        {
            scene_info = "This scenario contains plates as targets. On startup you will notice a round table at the center of the screen which is" +
                "responsible for hiding the plates. After the start count down, the taget plates will popup around the table one at a time.";
        } //Clay Pigeon * Rising Shape
        else if (simulator_scene_name.ToLower().Contains("clay pigeon") || simulator_scene_name.ToLower().Contains("rising shape"))
        {
            scene_info = "This scenario consists os plates as targets. After the start count down, the plates will popup for the bottom " +
                "of the screen moving upwards.";
        }
        else if (simulator_scene_name.ToLower().Contains("dice shoot"))
        {
            scene_info = "This scenario is a resemblance of a die. It consits of a 6 side cube with each side containg a number of plates." +
                " After the start count down, the die will roll and land on a random side that contains a ranom number of plates that you " +
                " will have to shoot and finish before the next roll.";
        }
        else if (simulator_scene_name.ToLower().Contains("shell game"))
        {
            scene_info = "This Scenario consists of upside down cups as targets. The cups will be shuffled, and you have to shoot the cup " +
                "that contains the white ball.";
        }
        else if (simulator_scene_name.ToLower().Contains("diverse bullseye"))
        {
            if (build_scene_name.ToLower().Contains("static"))
            {
                scene_info = "This scenario consists of a Bullseye IPEC Board. The objective is to hit the red point circle. If you miss, " +
                    "it will only be counted as a hit when the bullet lands within the point lines of the aimed bullseye circle. Note: " +
                    "Hitting the red point circle will add more points to your final score.";
            }
            else
            {
                scene_info = "This scenario consists of Bullseye IPEC Boards. When you hit the board it moves to another position.  " +
                    "The objective is to hit the red point circle. If you miss, it will only be counted as a hit when the bullet lands" +
                    " within the point lines of the aimed bullseye circle. Note: Hitting the red point circle will add more points to your" +
                    " final score.";
            }
        }
        else if (simulator_scene_name.ToLower().Contains("dueling tree"))
        {
            scene_info = "THis scenario consists of plates as targets. You have a pole at the center, and it consits of three plate on each " +
                "side. When you hit a plate on one side, it flips over to another side.";
        }
        else
        {
            scene_info = "No info.";
        }

        scene_info_dropdown.options.Add(new TMP_Dropdown.OptionData() { text = scene_info });

    }

}
