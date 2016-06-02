using UnityEngine;
using UnityEngine.UI;

public class Puntuaciones : MonoBehaviour
{
    public Text j_rojo;
    public Text j_azul;
    public Text last_winner;
    public Text rounds_played;

    void Start()
    {
        j_rojo.text = PlayerPrefs.GetInt("n_wins_rojo").ToString();
        j_azul.text = PlayerPrefs.GetInt("n_wins_azul").ToString();
        rounds_played.text = "Rondas jugadas: " + PlayerPrefs.GetInt("n_rondas_jugadas").ToString();

        if (PlayerPrefs.HasKey("last_winner") && PlayerPrefs.GetString("last_winner") == "Azul")
        {
            last_winner.text = "Ultimo ganador: Jugador Azul";
            last_winner.color = Color.blue;
        }
        else if(PlayerPrefs.HasKey("last_winner") && PlayerPrefs.GetString("last_winner") == "Rojo")
        {
            last_winner.text = "Último ganador: Jugador Rojo";
            last_winner.color = Color.red;
        }
        else
        {
            last_winner.text = "Ultimo ganador: -";
        }
    }

}