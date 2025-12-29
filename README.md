# 🐙 OctoBrief

**AI-Powered Web Monitoring & Email Summaries**

OctoBrief monitors websites you care about and sends you AI-generated summaries directly to your inbox. Never miss important news again—without the information overwhelm.

![Tech Stack](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5.6-3178C6?logo=typescript)
![TailwindCSS](https://img.shields.io/badge/Tailwind-3.4-38B2AC?logo=tailwind-css)

## ✨ Features

- 🌐 **Monitor Any Website** - News sites, blogs, social media pages, or any web content
- 🤖 **AI Summarization** - Get concise summaries of main headlines using OpenAI
- 📧 **Email Delivery** - Receive updates daily, weekly, or monthly
- ⏰ **Automated Scheduling** - Cron-based background jobs with Hangfire
- 📊 **User Dashboard** - Manage preferences and view past summaries
- 🗄️ **Minimal Database** - Lightweight SQLite storage for preferences
- 🎨 **Modern UI** - Clean, responsive design with React 19 and Tailwind CSS

## 🏗️ Architecture

```
├── src/
│   ├── OctoBrief.Api/          # .NET 10 Backend
│   │   ├── Controllers/        # API endpoints
│   │   ├── Services/           # Business logic (scraping, AI, email)
│   │   ├── Models/             # Data models
│   │   ├── Data/               # EF Core DbContext
│   │   └── DTOs/               # Data transfer objects
│   └── OctoBrief.Client/       # React 19 Frontend
│       ├── src/
│       │   ├── pages/          # HomePage & DashboardPage
│       │   ├── api.ts          # API client
│       │   └── types.ts        # TypeScript interfaces
│       └── public/
├── docker-compose.yml
└── OctoBrief.sln
```

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or .NET 9 as fallback)
- [Node.js 20+](https://nodejs.org/)
- OpenAI API Key (get from [platform.openai.com](https://platform.openai.com))
- SMTP credentials (Gmail recommended)

### 1. Configure Environment

Create `src/OctoBrief.Api/appsettings.json`:

```json
{
  "OpenAI": {
    "ApiKey": "your-openai-api-key"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-gmail-app-password",
    "SenderName": "OctoBrief"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=octobrief.db"
  }
}
```

> **Gmail Users**: Use an [App Password](https://support.google.com/accounts/answer/185833), not your regular password.

### 2. Run Backend

```bash
cd src/OctoBrief.Api
dotnet restore
dotnet run --urls="http://localhost:5001"
```

The API will be available at:
- **API**: http://localhost:5001/api
- **Swagger**: http://localhost:5001/swagger
- **Hangfire Dashboard**: http://localhost:5001/hangfire

### 3. Run Frontend

In a new terminal:

```bash
cd src/OctoBrief.Client
npm install
npm run dev
```

The app will open at **http://localhost:5173**

## 📖 Usage

### Add a Monitoring Preference

1. Open http://localhost:5173
2. Enter:
   - Your email address
   - Website URL to monitor (e.g., `https://news.ycombinator.com`)
   - Optional website name
   - Frequency (Daily/Weekly/Monthly)
3. Click **"Start Monitoring"**

### View Dashboard

1. Click "My Dashboard" and enter your email
2. See all your monitoring preferences
3. View past summaries
4. Pause/resume or delete preferences
5. Trigger immediate monitoring

### Email Schedule

- **Daily**: Runs at 8:00 AM UTC every day
- **Weekly**: Runs Monday at 8:00 AM UTC
- **Monthly**: Runs on the 1st at 8:00 AM UTC

## 🐳 Docker Deployment

### 1. Setup Environment

```bash
cp .env.example .env
# Edit .env with your credentials
```

### 2. Run with Docker Compose

```bash
docker-compose up -d
```

Services will be available at:
- Frontend: http://localhost:5173
- Backend: http://localhost:5001

### 3. Stop Services

```bash
docker-compose down
```

## 🔧 Configuration

### Backend (`appsettings.json`)

| Setting | Description | Default |
|---------|-------------|---------|
| `OpenAI:ApiKey` | OpenAI API key for summarization | Required |
| `Email:SmtpHost` | SMTP server hostname | smtp.gmail.com |
| `Email:SmtpPort` | SMTP server port | 587 |
| `Email:SenderEmail` | Sender email address | Required |
| `Email:SenderPassword` | Email password/app password | Required |
| `ConnectionStrings:DefaultConnection` | SQLite database path | Data Source=octobrief.db |

### Frontend (`vite.config.ts`)

The frontend proxies API requests to the backend. Update the proxy target if backend runs on a different port:

```typescript
proxy: {
  '/api': {
    target: 'http://localhost:5001', // Change if needed
    changeOrigin: true,
  },
}
```

## 📊 API Endpoints

### Preferences

- `GET /api/preferences?email={email}` - Get all preferences for an email
- `POST /api/preferences` - Create new monitoring preference
- `PUT /api/preferences/{id}` - Update preference
- `DELETE /api/preferences/{id}` - Delete preference
- `POST /api/preferences/{id}/trigger` - Manually trigger monitoring

### Summaries

- `GET /api/summaries?email={email}` - Get all summaries for an email
- `GET /api/summaries/{id}` - Get specific summary
- `POST /api/summaries/preview` - Preview summary for a URL

## 🧰 Tech Stack

### Backend
- **.NET 10** - Modern C# framework
- **Entity Framework Core** - ORM with SQLite
- **Hangfire** - Background job processing
- **HtmlAgilityPack** - Web scraping
- **OpenAI SDK** - AI summarization
- **MailKit** - Email sending

### Frontend
- **React 19** - Latest React with new features
- **TypeScript** - Type-safe JavaScript
- **Tailwind CSS** - Utility-first styling
- **TanStack Query** - Data fetching & caching
- **Vite** - Fast build tool
- **React Router** - Client-side routing
- **Lucide Icons** - Beautiful icons

## 🛠️ Development

### Run Tests

```bash
# Backend
cd src/OctoBrief.Api
dotnet test

# Frontend
cd src/OctoBrief.Client
npm test
```

### Database Migrations

```bash
cd src/OctoBrief.Api
dotnet ef migrations add MigrationName
dotnet ef database update
```

### View Hangfire Dashboard

When the backend is running, visit http://localhost:5001/hangfire to:
- View scheduled jobs
- Monitor job execution
- Manually trigger jobs
- See job history and failures

## 🔍 Troubleshooting

### Backend Issues

**Port 5001 already in use:**
```bash
# Find and kill process
lsof -i :5001
kill -9 <PID>
# Or use different port
dotnet run --urls="http://localhost:5002"
```

**OpenAI API errors:**
- Verify your API key is correct
- Check your OpenAI account has available credits
- The app will use fallback summaries if OpenAI is unavailable

**Email sending fails:**
- For Gmail, use an App Password, not your regular password
- Enable "Less secure app access" if using regular password (not recommended)
- Check SMTP settings match your email provider

### Frontend Issues

**npm install fails:**
```bash
rm -rf node_modules package-lock.json
npm install
```

**Proxy errors:**
- Ensure backend is running before starting frontend
- Check `vite.config.ts` proxy target matches backend URL

## 📝 Notes

- The app uses SQLite for simplicity. For production, consider PostgreSQL or SQL Server.
- Web scraping may not work on all websites (JavaScript-heavy sites, rate limiting, etc.)
- Some websites block scrapers - respect robots.txt and terms of service
- The AI summaries require OpenAI API credits (costs apply)
- Email sending uses SMTP - ensure your provider allows it

## 🎯 Future Enhancements

- [ ] User authentication & multi-user support
- [ ] RSS feed support
- [ ] Custom summarization prompts
- [ ] Webhook notifications
- [ ] Mobile app
- [ ] Advanced scheduling options
- [ ] Content filtering & keywords
- [ ] Export summaries to PDF

## 📄 License

MIT License - feel free to use this for personal or commercial projects.

## 🤝 Contributing

Contributions welcome! Please open an issue or submit a PR.

---

Made with ❤️ using .NET 10 and React 19
