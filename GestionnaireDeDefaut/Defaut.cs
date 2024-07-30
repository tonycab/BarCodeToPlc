namespace GestionnaireDeDefaut
{
    public class Defaut
    {
		static public List<Defaut> defautlist = new List<Defaut>();


		public int NumeroDefaut { get; set; }
        public string Designation { get; set; }
        public int Niveau { get; set; }

        public Defaut(int numeroDefaut, string designation, int niveau)
        {
            NumeroDefaut = numeroDefaut;
            Designation = designation;
            Niveau = niveau;

        }

             
    }
}
