using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RotorCode : MonoBehaviour
{
    public float L_Blade;
    public float Rotor_A;
    private float pi=3.14f;


    public float windSpeed;
    [Tooltip("W")]
    public float Pwing;
    public float ro;


    [Header("Generator")]
    public double K;   //jeneratörün nominal güç deðeri (kW).
    public double Phi; //jeneratör manyetik akýsý (Wb).
    public double f;   //jeneratörün nominal frekans deðeri (Hz).
    public double En;  //jeneratörün nominal gerilim deðeri (V).
    public double H;   //jeneratör manyetik alan yoðunluðu (A/m).
    public double w;   //açýsal hýz (radyan/s).
    public double n;   //toplam harmonik sayýsý
    public double pn;  //kutup sayýsý
    public double Tn;  //nominal tork (Nm)
    public double t;   //zaman (s)
    public double voltage;

    [Header("Yüzdelik halinde 15% ise 15 yaz")]
    public float efficiency;

    public float turbunKaybi;//  <59.3%
    public float komsudanKayip;//typically 3-10%
    public float mechanicalLosses;//gearbox
    public float electricalLosses;
    public float percentage_of_time_out;//typically 2-3%

    [Tooltip("MW")]
    public float Poutput;

    public GameObject rotor;
    public float rotorRPM;

    [Header("Paneller")]
    public GameObject ekonomiPanel;
    public GameObject verimPanel;
    public GameObject gucPanel;
    [Header("TMP ler")]
    public TextMeshProUGUI windSpeedTXT;

    public TextMeshProUGUI turbunKaybiTXT;
    public TextMeshProUGUI komsudanKayipTXT;
    public TextMeshProUGUI mechanicalLossesTXT;
    public TextMeshProUGUI electricalLossesTXT;
    public TextMeshProUGUI percentage_of_time_outTXT;
    public TextMeshProUGUI efficiencyTXT;

    public TextMeshProUGUI PoutputTXT;
    public TextMeshProUGUI PwingTXT;

    public TextMeshProUGUI trBedeliTXT;
    public TextMeshProUGUI karTXT;
    public TextMeshProUGUI dolarKuruTXT;
    public TextMeshProUGUI kendiniCikarmaTXT;
    public TextMeshProUGUI toplamBedeliTXT;

    [Header("Para")]
    public float trBedeli;//TL/kWh
    public float kar;//TL/saatlik
    public float dolarKuru;
    public float kendiniCikarma;
    public float toplamBedeli;

    private void Awake()
    {
        if (Rotor_A == 0)
        {
            Rotor_A = pi * L_Blade * L_Blade;
        }

        if (komsudanKayip == 0)
        {
            komsudanKayip = 5f;
        }

        if (percentage_of_time_out == 0)
        {
            percentage_of_time_out = 2f;
        }
        if (electricalLosses == 0)
        {
            electricalLosses = 30.7217f;
        }
        if (ro == 0)
        {
            ro = 1.225f;
        }
        if (trBedeli == 0)
        {
            trBedeli = 2.2116f;
        }
        if (dolarKuru == 0)
        {
            dolarKuru = 19f;
        }

        efficiency = (1 - turbunKaybi / 100) * (1 - komsudanKayip / 100) * (1 - mechanicalLosses / 100) * (1 - electricalLosses / 100) * (1 - percentage_of_time_out / 100);

        uiDuzenle();

        ekonomiPanel.SetActive(false);
        verimPanel.SetActive(false);
        gucPanel.SetActive(false);
    }
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {


        efficiency = (1 - (turbunKaybi/100)) * (1 - (komsudanKayip / 100)) * (1 - (mechanicalLosses / 100)) * (1 - (electricalLosses / 100)) * (1 - (percentage_of_time_out / 100));


        Pwing = (0.5f) * ro * Rotor_A * windSpeed * windSpeed * windSpeed;

        try
        {
            voltage = generator.CharacteristicEquation(K, Phi, f, En, H, w, n, pn, Tn, t);
        }
        catch
        {

        }

        Poutput = (efficiency * Pwing)/1000000;

        PwingTXT.text = ((Pwing/ 1000000)+ " MW/h").ToString();
        PoutputTXT.text = (Poutput + " MW/h").ToString();

        rotorRPM = ((60 * windSpeed) / (2 * pi * L_Blade))*3;

        rotor.transform.Rotate(rotorRPM * 6 * Time.deltaTime, 0, 0);

        kar = Poutput * trBedeli * 1000;
        karTXT.text = (kar + " TL/hr").ToString();

        kendiniCikarma = toplamBedeli/((kar * 24) / dolarKuru);
        kendiniCikarmaTXT.text = (kendiniCikarma + " days").ToString();
    }

    public void windPP()
    {
        windSpeed++;
        windSpeedTXT.text = (windSpeed + " m/s").ToString();
    }
    public void windMM()
    {
        windSpeed--;
        windSpeedTXT.text = (windSpeed + " m/s").ToString();
    }
    public void uiDuzenle()
    {

        turbunKaybiTXT.text = (turbunKaybi + " %").ToString();
        komsudanKayipTXT.text = (komsudanKayip + " %").ToString();
        mechanicalLossesTXT.text = (mechanicalLosses + " %").ToString();
        electricalLossesTXT.text = (electricalLosses + " %").ToString();
        percentage_of_time_outTXT.text = (percentage_of_time_out + " %").ToString();

        efficiencyTXT.text = ((efficiency*100) + " %").ToString();

        trBedeliTXT.text = (trBedeli + " TL/kWh").ToString();
        dolarKuruTXT.text = (dolarKuru + " TL/USD").ToString();
        toplamBedeliTXT.text = (toplamBedeli + " USD").ToString();


    }


    public void ekonomiBut()
    {
        ekonomiPanel.SetActive(true);
        verimPanel.SetActive(false);
        gucPanel.SetActive(false);
    }
    public void verimBut()
    {
        ekonomiPanel.SetActive(false);
        verimPanel.SetActive(true);
        gucPanel.SetActive(false);
    }
    public void gucBut()
    {
        ekonomiPanel.SetActive(false);
        verimPanel.SetActive(false);
        gucPanel.SetActive(true);
    }



}
