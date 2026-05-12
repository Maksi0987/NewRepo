using System;
using System.Drawing;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


public partial class Form1 : Form
{
   
    private Label lblUrl;
    private TextBox txtUrl;
    private Label lblSearchQuery;
    private TextBox txtSearchQuery;
    private Button btnLoadAndParse;
    private TextBox txtResult;

   
    public Form1()
    {
        InitializeComponent();
        InitializeCustomComponents(); 
    }


    private void InitializeComponent()
    {
       
        this.SuspendLayout();
      
        this.Text = "HTTP Парсер (Екзамен)";
        this.ClientSize = new Size(600, 450);
       
        this.ResumeLayout(false);
    }

    private void InitializeCustomComponents()
    {

        lblUrl = new Label { Text = "URL для завантаження:", Location = new Point(10, 10), AutoSize = true };
        txtUrl = new TextBox { Name = "txtUrl", Location = new Point(10, 30), Size = new Size(580, 20) };
      
        txtUrl.Text = "https://example.com";

       
        lblSearchQuery = new Label { Text = "Рядок для пошуку (запит):", Location = new Point(10, 60), AutoSize = true };
        txtSearchQuery = new TextBox { Name = "txtSearchQuery", Location = new Point(10, 80), Size = new Size(450, 20) };
       
        txtSearchQuery.Text = "domain";


        btnLoadAndParse = new Button { Text = "Завантажити та Парсити", Name = "btnLoadAndParse", Location = new Point(470, 78), Size = new Size(120, 25) };
        btnLoadAndParse.Click += new EventHandler(btnLoadAndParse_Click);

     
        txtResult = new TextBox
        {
            Name = "txtResult",
            Location = new Point(10, 120),
            Size = new Size(580, 320),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true
        };


        this.Controls.Add(lblUrl);
        this.Controls.Add(txtUrl);
        this.Controls.Add(lblSearchQuery);
        this.Controls.Add(txtSearchQuery);
        this.Controls.Add(btnLoadAndParse);
        this.Controls.Add(txtResult);
    }



    private async void btnLoadAndParse_Click(object sender, EventArgs e)
    {
        string url = txtUrl.Text.Trim();
        string query = txtSearchQuery.Text.Trim();

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(query))
        {
            MessageBox.Show("Введіть коректний URL та рядок для пошуку.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtResult.Text = $"🚀 Завантаження: {url}\r\n";

        try
        {
      
            using (HttpClient client = new HttpClient())
            {
             
                string htmlContent = await client.GetStringAsync(url);
                txtResult.Text += $"✅ Завантажено. Розмір HTML: {htmlContent.Length} символів. \r\n\r\n";

                int count = CountOccurrences(htmlContent, query);

        
                Regex paragraphRegex = new Regex($@"<p\b[^>]*>(.*?{Regex.Escape(query)}.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection paragraphMatches = paragraphRegex.Matches(htmlContent);

                string resultText = $"*** Результати запиту '{query}' ***\r\n";
                resultText += $"1. Кількість прямих входжень рядка: **{count}** раз(ів)\r\n";
                resultText += $"2. Знайдено абзаців (<p>), що містять запит: **{paragraphMatches.Count}**\r\n\r\n";

                if (paragraphMatches.Count > 0)
                {
                    resultText += "Перші 3 знайдені уривки:\r\n";
                    for (int i = 0; i < Math.Min(3, paragraphMatches.Count); i++)
                    {

                        string snippet = paragraphMatches[i].Groups[1].Value.Trim();
                
                        resultText += $"- {snippet.Substring(0, Math.Min(snippet.Length, 100))}...\r\n";
                    }
                }

                txtResult.Text += resultText;
            }
        }
        catch (HttpRequestException)
        {
            txtResult.Text += "❌ Помилка HTTP-запиту. Перевірте URL та підключення.";
        }
        catch (Exception ex)
        {
            txtResult.Text += $"❌ Сталася непередбачена помилка: {ex.Message}";
        }
    }

    
    private int CountOccurrences(string text, string pattern)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern)) return 0;

        int count = 0;
        int i = text.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
        while (i != -1)
        {
            count++;
            i = text.IndexOf(pattern, i + pattern.Length, StringComparison.OrdinalIgnoreCase);
        }
        return count;
    }
}