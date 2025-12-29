### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd OctoBrief
   ```

2. **Configure API Settings**
   
   Open `src/OctoBrief.Api/appsettings.json` and add your credentials:
   
   ```json
   {
      "Email": {
        "SmtpHost": "smtp.mail.yahoo.com", <-- You can use another provider if you want to, yahoo is the easiest to set up, go to login.yahoo.com/myaccount/security and enable your 2FA
        "SmtpPort": 587,
        "SenderEmail": "REPLACE_HERE",
        "SenderPassword": "REPLACE_HERE",
        "SenderName": "OctoBrief"
      },
     "OpenAI": {
       "ApiKey": "REPLACE_HERE"
     }
   }
   ```

   **SMTP Settings Notes:**
   - For Gmail: Use an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password
   - For Yahoo: Use `smtp.mail.yahoo.com` with port `587`
   - For Outlook/Hotmail: Use `smtp-mail.outlook.com` with port `587`

3. **Install Frontend Dependencies**
   ```bash
   cd src/OctoBrief.Client
   npm install
   ```

4. **Run the Application**

   **Backend (Terminal 1):**
   ```bash
   cd src/OctoBrief.Api
   dotnet run
   ```
   The API will be available at `http://localhost:5000`

   **Frontend (Terminal 2):**
   ```bash
   cd src/OctoBrief.Client
   npm run dev
   ```
   The UI will be available at `http://localhost:5173`

5. **Start Using OctoBrief**
   - Open your browser to `http://localhost:5173`
   - Select a topic and country
   - Click "Generate Preview" to see the brief
   - Enter your email and click "Send to Email" to receive it
