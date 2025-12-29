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
     "OpenAI": {
       "ApiKey": "sk-your-openai-api-key-here"
     },
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "your-email@gmail.com",
       "Password": "your-app-password-here",
       "FromEmail": "your-email@gmail.com",
       "FromName": "OctoBrief"
     }
   }
   ```

   **SMTP Settings Notes:**
   - For Gmail: Use an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password
   - For Yahoo: Use `smtp.mail.yahoo.com` with port `587`
   - For Outlook/Hotmail: Use `smtp-mail.outlook.com` with port `587`
   - Make sure to enable "Less secure app access" or use app-specific passwords

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

### Configuration Details

**OpenAI API Key:**
- Sign up at [platform.openai.com](https://platform.openai.com)
- Go to API Keys section and create a new key
- Copy the key (starts with `sk-`) to `appsettings.json`

**SMTP Configuration:**
- The app uses SMTP to send emails with news briefs
- Port 587 uses STARTTLS encryption (recommended)
- Test your credentials before using in production