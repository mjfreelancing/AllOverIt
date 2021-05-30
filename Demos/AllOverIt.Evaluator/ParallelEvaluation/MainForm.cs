using System;
using System.Windows.Forms;

namespace ParallelEvaluation
{
  public partial class FrmMain : Form
  {
    private readonly BitmapFactory[] _factories =
    {
      new BitmapFactory("cos(x)", "tan(x*y)", "sin(x^2-y^2)"),
      new BitmapFactory("(x^2)%(y+1)", "y%(x+1)", "x%(y+1)"),
      new BitmapFactory("cos(x/3.14)", "tan(x*3.14/180*y)", "sin(x^2-y^2)"),
      new BitmapFactory("sin(x)", "cos(y)", "tanh(x^2/y^2)")
    };

    private int _nextIdx;

    public FrmMain()
    {
      InitializeComponent();
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
      var factory = _factories[_nextIdx++];

      if (_nextIdx == _factories.Length)
      {
        _nextIdx = 0;
      }

      long elapsedMilliseconds;
      var bitmap = factory.CreateBitmap(pnlImage.Width, pnlImage.Height, out elapsedMilliseconds);

      var calcsPerformed = 3L * pnlImage.Width * pnlImage.Height;
      var calcRate = calcsPerformed * 1000 / elapsedMilliseconds;
      lblCalcRate.Text = $"Status: {calcsPerformed} calcs in {elapsedMilliseconds}ms = {calcRate} / sec";

      pnlImage.CreateGraphics().DrawImage(bitmap, 0, 0);
    }
  }
}
