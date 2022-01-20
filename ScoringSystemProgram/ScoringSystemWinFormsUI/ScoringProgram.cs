﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ScoringSystemWinFormsUI
{
    public partial class ScoringProgram : Form
    {
        #region Constructor

        public ScoringProgram()
        {
            InitializeComponent();
        }

        #endregion

        #region Declaring Score Storage Dictionaries

        // Creating the eventScores and totalScores dictionaries 
        public IDictionary<string, int[]> eventScores = new Dictionary<string, int[]>();
        public IDictionary<string, int> totalScores = new Dictionary<string, int>();

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Clears all of the data in the eventScores and totalScores dictionaries by assigning them new, empty dicts
        /// </summary>
        /// <param name="eventScores">The dictionary containing the event scores</param>
        /// <param name="totalScores">The dictionary containing the total scores</param>
        
        private void ClearScoreDictionaries(ref Dictionary<string, int[]> eventScores, ref Dictionary<string, int> totalScores)
        {
            // Overwrites data in dicts by assigning them empty dicts
            eventScores = new Dictionary<string, int[]>();
            totalScores = new Dictionary<string, int>();
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// If a user changes tab, this code runs. It checks which tab is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Once the selected tab is changed, check if it's the output tab
            if (tabControl.SelectedTab == tabControl.TabPages["outputTab"])
            {
                // Loop through each entry in the eventScores dictionary
                foreach (KeyValuePair<string, int[]> entry in eventScores)
                {
                    string tKey = entry.Key; // Temporary variable to store name
                    int tValue = 0; // Temporary variable to store total score

                    // Loop over each of the current competitor's scores
                    // This loop sums all of the event scores into a total
                    for (int i = 0; i < entry.Value.Length; i++)
                    {
                        // Add the score for that event to the total
                        tValue += entry.Value[i];
                    }

                    // If the key is already in the totalScores dictionary
                    if (totalScores.ContainsKey(tKey))
                    {
                        // Update the value 
                        totalScores[tKey] = tValue;
                    }

                    else // If the key isn't already in totalScores
                    {
                        // Add the temporary key and value to total scores
                        totalScores.Add(new KeyValuePair<string, int>(tKey, tValue));
                    }
                }

                // ***TODO*** Make insertion sort method          

                // Clear table before writing data from totalScores to outputTable
                // Doing this because it prevents duplicated from being added
                outputTable.DataSource = null;

                // ***NOTE*** This only works if totalScores and the outputTable are in the same row

                // Loop over each row of the output table
                for (int i = 0; i < totalScores.Count; i++)
                {
                    // Get key and value from totalScores to use for comparisons
                    string currentKey = totalScores.ElementAt(i).Key;
                    string currentValue = totalScores.ElementAt(i).Value.ToString();

                    // If the current key is found in the name column of the output table
                    if (currentKey == (string)outputTable.Rows[i].Cells[0].Value)
                    {
                        // Set the total score column to the current value... a.k.a current total score
                        outputTable.Rows[i].Cells[1].Value = currentValue;
                    }
                    
                    else // Else if the key isn't isn't in the name column of the output table
                    {
                        // Create a new array that contains the name and total score of a competitor
                        // Add the row to the DataGridView representing the output table
                        string[] newRow = new string[2] { currentKey.ToString(), currentValue.ToString() };
                        outputTable.Rows.Add(newRow);
                    }
                }
            }
        }

        /// <summary>
        /// This code is run if the user clicks the enter button on the input tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void enterInputButton_Click(object sender, EventArgs e)
        {
            // Store the inputs in some variables
            string name = nameInputTextBox.Text;
            int rank = int.Parse(rankInputTextBox.Text); // ***TODO*** Change int.Parse to int.TryParse
            int eventNum = eventInputComboBox.SelectedIndex; // ***NOTE*** Maybe selectedIndex - 1? Come back to this

            // Set flagging variable, tells us if the name exists in the dictionary
            bool exists = false;

            // Loop through each key value pair in the eventScores dictionary
            foreach (KeyValuePair<string, int[]> entry in eventScores)
            {
                // If name exists
                if (name == entry.Key)
                {
                    // Calculate rank and store in correct col
                    // Set flag to true so we don't run the if statement that adds a new key 
                    // Then break out of the loop because we already found the right key
                    entry.Value[eventNum] = 11 - rank;
                    exists = true;
                    break;
                }
            }

            if (!exists) // If the name isn't a key in eventScores... exists != true
            {
                // Create a new string with the variable 'name'
                // Create new array[] with length 5, each item is 0
                string nKey = name;
                int[] nValue = new int[5] { 0, 0, 0, 0, 0 };

                // Set the value for the event to 11 - rank... set nValue[eventNum] to 11 - rank
                // Then add the new key and new value to the eventScores dictionary
                nValue[eventNum] = 11 - rank;
                eventScores.Add(new KeyValuePair<string, int[]>(nKey, nValue));

            }

            // Finally, reset text boxes. 
            // Reset combo box to unselected value
            // ***NOTE*** Come back to this
            nameInputTextBox.Text = string.Empty;
            rankInputTextBox.Text = string.Empty;
            eventInputComboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// This is run if the user clicks 'Write to File.' Opens a new window prompting the user to input a path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void writeToFileButton_Click(object sender, EventArgs e)
        {
            // Instantiate the popup box form
            // Then use writeToFilePopup.Show(); to show the popup
            // Passes in totalScores to the constructor, so we can use it in the write to file form 
            WriteToFilePopup writeToFilePopup = new WriteToFilePopup(totalScores);
            writeToFilePopup.Show();       
        }

        #endregion
    }
}
