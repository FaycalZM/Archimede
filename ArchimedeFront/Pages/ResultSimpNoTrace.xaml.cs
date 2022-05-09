﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Archimède;

namespace ArchimedeFront.Pages
{
    /// <summary>
    /// Interaction logic for ResultSimpNoTrace.xaml
    /// </summary>
    public partial class ResultSimpNoTrace : Page
    {
        public ResultSimpNoTrace()
        {
            InitializeComponent();
            if (Data.literal)
            {



                int maxNbUns = Data.stringListMinterm.MaxBy(x => x.Count(ch => (ch == '1' || ch == '-'))).Count(ch => (ch == '1' || ch == '-'));

                foreach (string mintermBinCode in Data.stringListMinterm)
                {
                    Impliquant impliquant = new Impliquant(mintermBinCode);
                    if (impliquant.nbDontCare > 0) Data.impliquantsEnAttente.Add(impliquant); // ces impliquants vont etre traités dans les prochaine groupe 
                    else (Data.impliquants).Add(impliquant); // impliquants  en forme canonique 
                }

                Data.groupeMintermes = new Mintermes(maxNbUns);
               /* for (int i = 0; i < Data.stringListMinterm.Count; i++)
                {
                    // mintermes[i] = string.Join(".", mintermes[i].Split(".").ToList().OrderBy(var => { if (var[0] == '!') return var[1..]; return var; }).ToList());
                    mintermes.Add(Minterme.bincodeToMinterm(Data.stringListMinterm[i], Data.variables));
                */

               // Data.expressionTransforme = string.Join(" + ", mintermes);

               // expression.Text = Data.expressionTransforme;



            }
            else
            {

                //Corriger les codes binaires (en ajoutant des zéros au début pour qu'ils aient tous la mê^me longueur)
                for (int i = 0; i < Data.mintermes.Count; i++)
                {
                    Data.mintermes[i].bincode = Data.mintermes[i].bincode.PadLeft(Data.nbVariables, '0');
                    Data.stringListMinterm.Add(Data.mintermes[i].bincode);

                }
                Data.groupeMintermes = new Mintermes(Minterme.maxNbUns);
                Data.impliquants = Data.groupeMintermes.InitImpliquants(Data.mintermes);
                Data.stringListMinterm = Data.stringListMinterm.Distinct().ToList();
                //mintermes = Data.listMintermesString;

                //expression.Text = string.Join(" ,", Data.listMintermesString);
            }
            Data.impliquantsEssentiels = Mintermes.simplifyMintermes(Data.impliquants, Data.impliquantsEnAttente, Data.groupeMintermes, Data.stringListMinterm, Data.literal);
            string resultat = Mintermes.getResultatExpressionDNF(Data.literal, Data.impliquantsEssentiels, Data.variables);
            if (resultat.Length == 0) FonctionSimplifie.Text = "VRAI";
            else FonctionSimplifie.Text = resultat;


        }
    }
}
