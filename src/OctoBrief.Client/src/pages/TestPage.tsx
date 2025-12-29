import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Octagon, ArrowLeft, CheckCircle, XCircle, AlertCircle, Loader2, Globe, Brain, Mail, Zap } from 'lucide-react'
import { api, ConfigStatus, ScrapeTestResult, AiTestResult, EmailTestResult, FullTestResult } from '../api'

type TestStatus = 'idle' | 'running' | 'success' | 'error' | 'warning'

export function TestPage() {
  const navigate = useNavigate()
  const [configStatus, setConfigStatus] = useState<ConfigStatus | null>(null)
  const [testUrl, setTestUrl] = useState('https://news.ycombinator.com')
  const [testEmail, setTestEmail] = useState('')

  // Test results
  const [scrapeResult, setScrapeResult] = useState<ScrapeTestResult | null>(null)
  const [scrapeStatus, setScrapeStatus] = useState<TestStatus>('idle')
  
  const [aiResult, setAiResult] = useState<AiTestResult | null>(null)
  const [aiStatus, setAiStatus] = useState<TestStatus>('idle')
  
  const [emailResult, setEmailResult] = useState<EmailTestResult | null>(null)
  const [emailStatus, setEmailStatus] = useState<TestStatus>('idle')
  
  const [fullResult, setFullResult] = useState<FullTestResult | null>(null)
  const [fullStatus, setFullStatus] = useState<TestStatus>('idle')

  useEffect(() => {
    loadConfig()
  }, [])

  const loadConfig = async () => {
    try {
      const config = await api.getConfigStatus()
      setConfigStatus(config)
    } catch (err) {
      console.error('Failed to load config:', err)
    }
  }

  const runScrapeTest = async () => {
    setScrapeStatus('running')
    setScrapeResult(null)
    try {
      const result = await api.testScrape(testUrl)
      setScrapeResult(result)
      setScrapeStatus(result.success ? 'success' : 'error')
    } catch (err: any) {
      setScrapeResult({ success: false, message: err.message, error: 'Network Error', headlinesFound: 0, headlines: [], contentLength: 0, elapsedMs: 0 })
      setScrapeStatus('error')
    }
  }

  const runAiTest = async () => {
    setAiStatus('running')
    setAiResult(null)
    try {
      const result = await api.testAi(testUrl)
      setAiResult(result)
      setAiStatus(result.success ? (result.usingFallback ? 'warning' : 'success') : 'error')
    } catch (err: any) {
      setAiResult({ success: false, message: err.message, error: 'Network Error', usingFallback: false, headlineCount: 0, headlines: [], scrapeElapsedMs: 0, aiElapsedMs: 0 })
      setAiStatus('error')
    }
  }

  const runEmailTest = async () => {
    if (!testEmail) {
      alert('Please enter an email address')
      return
    }
    setEmailStatus('running')
    setEmailResult(null)
    try {
      const result = await api.testEmail(testEmail)
      setEmailResult(result)
      setEmailStatus(result.success ? 'success' : 'error')
    } catch (err: any) {
      setEmailResult({ success: false, message: err.message, error: 'Network Error', elapsedMs: 0 })
      setEmailStatus('error')
    }
  }

  const runFullTest = async () => {
    setFullStatus('running')
    setFullResult(null)
    try {
      const result = await api.testFullPipeline(testUrl, testEmail || undefined)
      setFullResult(result)
      const hasError = result.steps.some(s => s.status === 'error')
      const hasWarning = result.steps.some(s => s.status === 'warning')
      setFullStatus(hasError ? 'error' : hasWarning ? 'warning' : 'success')
    } catch (err: any) {
      setFullResult({ success: false, message: err.message, steps: [] })
      setFullStatus('error')
    }
  }

  const StatusIcon = ({ status }: { status: TestStatus }) => {
    switch (status) {
      case 'running':
        return <Loader2 className="w-5 h-5 text-blue-500 animate-spin" />
      case 'success':
        return <CheckCircle className="w-5 h-5 text-green-500" />
      case 'error':
        return <XCircle className="w-5 h-5 text-red-500" />
      case 'warning':
        return <AlertCircle className="w-5 h-5 text-yellow-500" />
      default:
        return <div className="w-5 h-5 rounded-full border-2 border-gray-300" />
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-50 via-white to-blue-50">
      {/* Header */}
      <header className="border-b bg-white/80 backdrop-blur-sm sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <Octagon className="w-8 h-8 text-primary-600" />
              <h1 className="text-2xl font-bold text-gray-900">OctoBrief - Debug & Test</h1>
            </div>
            <button
              onClick={() => navigate('/')}
              className="text-primary-600 hover:text-primary-700 font-medium flex items-center gap-2"
            >
              <ArrowLeft className="w-4 h-4" />
              Back to Home
            </button>
          </div>
        </div>
      </header>

      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        {/* Configuration Status */}
        <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 mb-8">
          <h2 className="text-xl font-bold text-gray-900 mb-4">Configuration Status</h2>
          {configStatus ? (
            <div className="grid md:grid-cols-2 gap-4">
              <div className={`p-4 rounded-xl ${configStatus.openAiConfigured ? 'bg-green-50 border border-green-200' : 'bg-red-50 border border-red-200'}`}>
                <div className="flex items-center gap-2">
                  {configStatus.openAiConfigured ? <CheckCircle className="w-5 h-5 text-green-600" /> : <XCircle className="w-5 h-5 text-red-600" />}
                  <span className="font-medium">OpenAI API</span>
                </div>
                <p className="text-sm text-gray-600 mt-1">
                  {configStatus.openAiConfigured ? 'API key configured' : 'Not configured - using fallback summaries'}
                </p>
              </div>
              <div className={`p-4 rounded-xl ${configStatus.emailConfigured ? 'bg-green-50 border border-green-200' : 'bg-red-50 border border-red-200'}`}>
                <div className="flex items-center gap-2">
                  {configStatus.emailConfigured ? <CheckCircle className="w-5 h-5 text-green-600" /> : <XCircle className="w-5 h-5 text-red-600" />}
                  <span className="font-medium">Email (SMTP)</span>
                </div>
                <p className="text-sm text-gray-600 mt-1">
                  {configStatus.emailConfigured ? `${configStatus.smtpHost}:${configStatus.smtpPort}` : 'Not configured - emails will be logged only'}
                </p>
              </div>
            </div>
          ) : (
            <div className="flex items-center gap-2 text-gray-500">
              <Loader2 className="w-5 h-5 animate-spin" />
              Loading configuration...
            </div>
          )}
        </div>

        {/* Test Inputs */}
        <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 mb-8">
          <h2 className="text-xl font-bold text-gray-900 mb-4">Test Parameters</h2>
          <div className="grid md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Test URL</label>
              <input
                type="url"
                value={testUrl}
                onChange={(e) => setTestUrl(e.target.value)}
                placeholder="https://example.com"
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Test Email (for email test)</label>
              <input
                type="email"
                value={testEmail}
                onChange={(e) => setTestEmail(e.target.value)}
                placeholder="you@example.com"
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none"
              />
            </div>
          </div>
        </div>

        {/* Individual Tests */}
        <div className="space-y-6">
          {/* Test 1: Web Scraping */}
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <Globe className="w-6 h-6 text-primary-600" />
                <h3 className="text-lg font-bold text-gray-900">Test 1: Web Scraping</h3>
                <StatusIcon status={scrapeStatus} />
              </div>
              <button
                onClick={runScrapeTest}
                disabled={scrapeStatus === 'running'}
                className="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-lg transition disabled:opacity-50"
              >
                {scrapeStatus === 'running' ? 'Running...' : 'Run Test'}
              </button>
            </div>
            <p className="text-sm text-gray-600 mb-4">Tests fetching and parsing content from the target website.</p>
            
            {scrapeResult && (
              <div className={`p-4 rounded-xl ${scrapeResult.success ? 'bg-green-50' : 'bg-red-50'}`}>
                <p className={`font-medium ${scrapeResult.success ? 'text-green-800' : 'text-red-800'}`}>
                  {scrapeResult.message}
                </p>
                {scrapeResult.success && (
                  <div className="mt-3 space-y-2 text-sm text-gray-700">
                    <p><strong>Title:</strong> {scrapeResult.title}</p>
                    <p><strong>Headlines found:</strong> {scrapeResult.headlinesFound}</p>
                    <p><strong>Content length:</strong> {scrapeResult.contentLength} characters</p>
                    <p><strong>Time:</strong> {scrapeResult.elapsedMs}ms</p>
                    {scrapeResult.headlines.length > 0 && (
                      <div>
                        <strong>Sample headlines:</strong>
                        <ul className="list-disc list-inside mt-1">
                          {scrapeResult.headlines.map((h, i) => (
                            <li key={i} className="truncate">{h}</li>
                          ))}
                        </ul>
                      </div>
                    )}
                  </div>
                )}
                {scrapeResult.error && (
                  <p className="text-sm text-red-600 mt-2">Error: {scrapeResult.error}</p>
                )}
              </div>
            )}
          </div>

          {/* Test 2: AI Summarization */}
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <Brain className="w-6 h-6 text-primary-600" />
                <h3 className="text-lg font-bold text-gray-900">Test 2: AI Summarization</h3>
                <StatusIcon status={aiStatus} />
              </div>
              <button
                onClick={runAiTest}
                disabled={aiStatus === 'running'}
                className="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-lg transition disabled:opacity-50"
              >
                {aiStatus === 'running' ? 'Running...' : 'Run Test'}
              </button>
            </div>
            <p className="text-sm text-gray-600 mb-4">Scrapes the website and generates an AI summary using OpenAI.</p>
            
            {aiResult && (
              <div className={`p-4 rounded-xl ${aiResult.success ? (aiResult.usingFallback ? 'bg-yellow-50' : 'bg-green-50') : 'bg-red-50'}`}>
                <p className={`font-medium ${aiResult.success ? (aiResult.usingFallback ? 'text-yellow-800' : 'text-green-800') : 'text-red-800'}`}>
                  {aiResult.message}
                </p>
                {aiResult.usingFallback && (
                  <p className="text-sm text-yellow-700 mt-1">⚠️ Using fallback - OpenAI not configured or failed</p>
                )}
                {aiResult.success && (
                  <div className="mt-3 space-y-2 text-sm text-gray-700">
                    <p><strong>Overview:</strong> {aiResult.overview}</p>
                    <p><strong>Headlines summarized:</strong> {aiResult.headlineCount}</p>
                    <p><strong>Scrape time:</strong> {aiResult.scrapeElapsedMs}ms | <strong>AI time:</strong> {aiResult.aiElapsedMs}ms</p>
                    {aiResult.headlines.length > 0 && (
                      <div>
                        <strong>Summaries:</strong>
                        <div className="mt-2 space-y-2">
                          {aiResult.headlines.slice(0, 3).map((h, i) => (
                            <div key={i} className="bg-white p-3 rounded-lg border">
                              <p className="font-medium">{h.title}</p>
                              <p className="text-gray-600">{h.summary}</p>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                )}
                {aiResult.error && (
                  <p className="text-sm text-red-600 mt-2">Error: {aiResult.error}</p>
                )}
              </div>
            )}
          </div>

          {/* Test 3: Email */}
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <Mail className="w-6 h-6 text-primary-600" />
                <h3 className="text-lg font-bold text-gray-900">Test 3: Email Sending</h3>
                <StatusIcon status={emailStatus} />
              </div>
              <button
                onClick={runEmailTest}
                disabled={emailStatus === 'running' || !testEmail}
                className="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-lg transition disabled:opacity-50"
              >
                {emailStatus === 'running' ? 'Sending...' : 'Send Test Email'}
              </button>
            </div>
            <p className="text-sm text-gray-600 mb-4">Sends a test email to verify SMTP configuration.</p>
            
            {emailResult && (
              <div className={`p-4 rounded-xl ${emailResult.success ? 'bg-green-50' : 'bg-red-50'}`}>
                <p className={`font-medium ${emailResult.success ? 'text-green-800' : 'text-red-800'}`}>
                  {emailResult.message}
                </p>
                {emailResult.success && (
                  <p className="text-sm text-gray-700 mt-2">Sent to: {emailResult.sentTo} ({emailResult.elapsedMs}ms)</p>
                )}
                {emailResult.hint && (
                  <p className="text-sm text-yellow-700 mt-2 bg-yellow-100 p-2 rounded">💡 {emailResult.hint}</p>
                )}
                {emailResult.error && (
                  <p className="text-sm text-red-600 mt-2">Error type: {emailResult.error}</p>
                )}
              </div>
            )}
          </div>

          {/* Test 4: Full Pipeline */}
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <Zap className="w-6 h-6 text-primary-600" />
                <h3 className="text-lg font-bold text-gray-900">Test 4: Full Pipeline</h3>
                <StatusIcon status={fullStatus} />
              </div>
              <button
                onClick={runFullTest}
                disabled={fullStatus === 'running'}
                className="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-lg transition disabled:opacity-50"
              >
                {fullStatus === 'running' ? 'Running...' : 'Run Full Test'}
              </button>
            </div>
            <p className="text-sm text-gray-600 mb-4">Runs the complete pipeline: Scrape → AI → Email (if email provided)</p>
            
            {fullResult && (
              <div className={`p-4 rounded-xl ${fullResult.success ? 'bg-green-50' : 'bg-red-50'}`}>
                <p className={`font-medium ${fullResult.success ? 'text-green-800' : 'text-red-800'}`}>
                  {fullResult.message}
                </p>
                {fullResult.steps.length > 0 && (
                  <div className="mt-4 space-y-2">
                    {fullResult.steps.map((step, i) => (
                      <div key={i} className="flex items-center gap-3 p-3 bg-white rounded-lg border">
                        {step.status === 'success' && <CheckCircle className="w-5 h-5 text-green-500" />}
                        {step.status === 'error' && <XCircle className="w-5 h-5 text-red-500" />}
                        {step.status === 'warning' && <AlertCircle className="w-5 h-5 text-yellow-500" />}
                        {step.status === 'running' && <Loader2 className="w-5 h-5 text-blue-500 animate-spin" />}
                        <div className="flex-1">
                          <p className="font-medium">{step.name}</p>
                          {step.details && <p className="text-sm text-gray-600">{step.details}</p>}
                        </div>
                        {step.elapsedMs > 0 && <span className="text-sm text-gray-500">{step.elapsedMs}ms</span>}
                      </div>
                    ))}
                  </div>
                )}
                {fullResult.preview && (
                  <div className="mt-4 p-4 bg-white rounded-lg border">
                    <h4 className="font-medium text-gray-900">{fullResult.preview.title}</h4>
                    <p className="text-sm text-gray-700 mt-2">{fullResult.preview.overview}</p>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
