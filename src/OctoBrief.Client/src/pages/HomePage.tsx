import { useState } from 'react'
import { Octagon, Sparkles, Mail, Send, Loader2, CheckCircle, AlertCircle } from 'lucide-react'
import { api, PreviewBriefResponse } from '../api'

const COUNTRIES = [
  { value: 'romania', label: 'Romania' },
  { value: 'usa', label: 'USA' },
  { value: 'uk', label: 'UK' },
  { value: 'canada', label: 'Canada' },
  { value: 'germany', label: 'Germany' },
  { value: 'france', label: 'France' },
  { value: 'italy', label: 'Italy' },
  { value: 'spain', label: 'Spain' },
  { value: 'poland', label: 'Poland' },
]

const POPULAR_TOPICS = [
  'Technology', 'Science', 'Sports',
  'Media', 'Health', 'Climate', 'Politics', 'Crypto', 'Gaming'
]

export function HomePage() {
  const [topic, setTopic] = useState('');
  const [country, setCountry] = useState('');
  const [isLoading, setIsLoading] = useState(false)
  const [loadingStep, setLoadingStep] = useState('')
  const [result, setResult] = useState<PreviewBriefResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  
  // Email send state
  const [email, setEmail] = useState('')
  const [isSendingEmail, setIsSendingEmail] = useState(false)
  const [emailSent, setEmailSent] = useState(false)
  const [emailError, setEmailError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setResult(null)
    setIsLoading(true)
    setError(null)
    setEmailSent(false)
    setEmailError(null)

    if (!topic.trim()) {
      setError('Please enter a topic you are interested in')
      setIsLoading(false)
      return
    }

    try {
      setLoadingStep('Searching for news sources...')
      const response = await api.previewBrief(topic, country)
      
      if (response.success) {
        setResult(response)
      } else {
        setError(response.message || 'Failed to generate brief')
      }
    } catch (err: any) {
      setError(err.message || 'Failed to generate brief')
    } finally {
      setIsLoading(false)
      setLoadingStep('')
    }
  }

  const handleSendEmail = async () => {
    if (!email.trim() || !result?.htmlContent) return
    
    setIsSendingEmail(true)
    setEmailError(null)
    
    try {
      const response = await api.sendBriefToEmail(email, result.subject || 'Your News Brief', result.htmlContent)
      if (response.success) {
        setEmailSent(true)
      } else {
        setEmailError(response.message || 'Failed to send email')
      }
    } catch (err: any) {
      setEmailError(err.message || 'Failed to send email')
    } finally {
      setIsSendingEmail(false)
    }
  }

  const resetForm = () => {
    setTopic('')
    setCountry('global')
    setResult(null)
    setError(null)
    setEmail('')
    setEmailSent(false)
    setEmailError(null)
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b sticky top-0 z-10">
        <div className="max-w-6xl mx-auto px-4 py-3">
          <div className="flex items-center justify-center">
            <Octagon className={`text-primary-600 transition-all duration-500 ${
              isLoading 
                ? 'w-10 h-10 animate-spin-slow' 
                : 'w-8 h-8'
            }`} />
          </div>
        </div>
      </header>

      <main className="max-w-6xl mx-auto px-4 py-8">
        {/* Form Section */}
        {!result && !isLoading && (
          <div className="max-w-xl mx-auto">
            <div className="text-center mb-8">
              <h1 className="text-3xl font-bold text-gray-900 mb-2">
                Get Your News with OctoBrief
              </h1>
              <p className="text-gray-600">
                AI-powered news summaries tailored to your interests
              </p>
            </div>

            <div className="bg-white rounded-2xl shadow-lg p-6 md:p-8">
              {error && (
                <div className="mb-6 p-3 rounded-lg bg-red-50 border border-red-200 flex items-center gap-2">
                  <AlertCircle className="w-5 h-5 text-red-500 flex-shrink-0" />
                  <p className="text-red-700 text-sm">{error}</p>
                </div>
              )}

              <form onSubmit={handleSubmit} className="space-y-5">
                {/* Step 1: Topic */}
                <div>
                  <div className="flex items-center gap-2 mb-3">
                    <span className="w-6 h-6 rounded-full bg-primary-600 text-white text-xs font-bold flex items-center justify-center">1</span>
                    <label className="font-medium text-gray-900">What topic?</label>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    {POPULAR_TOPICS.map((t) => (
                      <button
                        key={t}
                        type="button"
                        onClick={() => setTopic(t)}
                        className={`px-3 py-1.5 text-sm rounded-lg transition ${
                          topic.toLowerCase() === t.toLowerCase()
                            ? 'bg-primary-600 text-white'
                            : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                        }`}
                      >
                        {t}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Step 2: Region */}
                <div>
                  <div className="flex items-center gap-2 mb-3">
                    <span className="w-6 h-6 rounded-full bg-primary-600 text-white text-xs font-bold flex items-center justify-center">2</span>
                    <label className="font-medium text-gray-900">What region?</label>
                  </div>
                  <div className="grid grid-cols-5 gap-2">
                    {COUNTRIES.map((c) => (
                      <button
                        key={c.value}
                        type="button"
                        onClick={() => setCountry(c.value)}
                        className={`px-2 py-2 text-sm rounded-lg transition text-center ${
                          country === c.value
                            ? 'bg-primary-600 text-white'
                            : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                        }`}
                      >
                        {c.label}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Submit */}
                <button
                  type="submit"
                  disabled={!topic.trim()}
                  className="w-full bg-primary-600 hover:bg-primary-700 text-white font-semibold py-4 rounded-xl transition disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 text-lg"
                >
                  <Sparkles className="w-5 h-5" />
                  Generate Brief
                </button>
              </form>
            </div>
          </div>
        )}

        {/* Loading State */}
        {isLoading && (
          <div className="max-w-xl mx-auto">
            <div className="bg-white rounded-2xl shadow-lg p-8">
              <div className="text-center">
                <h3 className="text-lg font-semibold text-gray-900 mb-1">
                  Searching {topic} in {COUNTRIES.find(c => c.value === country)?.label || 'Global'}
                </h3>
                <p className="text-gray-500 text-sm">{loadingStep || 'This may take 15-30 seconds...'}</p>
              </div>
            </div>
          </div>
        )}

        {/* Results Display */}
        {result && result.success && (
          <div className="space-y-4">
            {/* Header with New Search */}
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <button
                  onClick={resetForm}
                  className="text-gray-400 hover:text-gray-600 transition"
                  title="New Search"
                >
                  ←
                </button>
                <div>
                  <h1 className="text-xl font-bold text-gray-900">
                    {topic} News
                  </h1>
                  <p className="text-gray-500 text-xs">
                    {result.websiteResults?.filter(w => w.success).length || 0} sources • {COUNTRIES.find(c => c.value === country)?.label}
                  </p>
                </div>
              </div>
            </div>

            {/* News Cards */}
            <div 
              className="news-grid"
              dangerouslySetInnerHTML={{ __html: result.htmlContent || '' }}
            />

            {/* Email Section */}
            <div className="bg-white rounded-lg shadow-sm border p-3 mt-4">
              <div className="flex items-center gap-3">
                <Mail className="w-4 h-4 text-gray-400 flex-shrink-0" />
                
                {!emailSent ? (
                  <>
                    <input
                      type="email"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      placeholder="Send to email..."
                      className="flex-1 px-3 py-1.5 border border-gray-200 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none transition text-sm"
                    />
                    <button
                      onClick={handleSendEmail}
                      disabled={isSendingEmail || !email.trim()}
                      className="px-3 py-1.5 bg-primary-600 hover:bg-primary-700 text-white text-sm font-medium rounded-lg transition disabled:opacity-50 flex items-center gap-1.5"
                    >
                      {isSendingEmail ? (
                        <Loader2 className="w-3.5 h-3.5 animate-spin" />
                      ) : (
                        <Send className="w-3.5 h-3.5" />
                      )}
                      Send
                    </button>
                  </>
                ) : (
                  <div className="flex items-center gap-2 text-green-600">
                    <CheckCircle className="w-4 h-4" />
                    <span className="text-sm font-medium">Sent!</span>
                  </div>
                )}
              </div>
              {emailError && <p className="mt-2 text-sm text-red-600 pl-7">{emailError}</p>}
            </div>
          </div>
        )}

        {/* Error State */}
        {result && !result.success && (
          <div className="max-w-xl mx-auto">
            <div className="bg-white rounded-2xl shadow-lg p-8 text-center">
              <AlertCircle className="w-12 h-12 text-red-500 mx-auto mb-4" />
              <h3 className="text-xl font-bold text-gray-900 mb-2">Something went wrong</h3>
              <p className="text-gray-600 mb-6">{result.message}</p>
              <button
                onClick={resetForm}
                className="px-6 py-3 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-xl transition"
              >
                Try Again
              </button>
            </div>
          </div>
        )}
      </main>

      {/* Styles for news content */}
      <style>{`
        .news-grid {
          display: grid;
          grid-template-columns: repeat(3, 1fr);
          gap: 0.75rem;
        }
        @media (max-width: 1024px) {
          .news-grid {
            grid-template-columns: repeat(2, 1fr);
          }
        }
        @media (max-width: 640px) {
          .news-grid {
            grid-template-columns: 1fr;
          }
        }
        .news-grid > h2,
        .news-grid > .intro,
        .news-grid > .logo,
        .news-grid > .footer,
        .news-grid .source-section > h3 {
          display: none;
        }
        .news-grid .source-section {
          display: contents;
        }
        .news-grid .story {
          background: white;
          border-radius: 10px;
          padding: 1rem;
          box-shadow: 0 1px 3px rgba(0,0,0,0.08);
          border: 1px solid #e5e7eb;
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          min-height: 110px;
          transition: box-shadow 0.15s, transform 0.15s;
        }
        .news-grid .story:hover {
          box-shadow: 0 4px 12px rgba(0,0,0,0.1);
          transform: translateY(-1px);
        }
        .news-grid .story h4 {
          font-size: 0.875rem;
          font-weight: 600;
          color: #111827;
          line-height: 1.4;
          margin: 0;
          display: -webkit-box;
          -webkit-line-clamp: 3;
          -webkit-box-orient: vertical;
          overflow: hidden;
          flex: 1;
        }
        .news-grid .story p {
          display: none;
        }
        .news-grid .story-link {
          display: inline-flex;
          align-items: center;
          gap: 0.375rem;
          font-size: 0.6875rem;
          color: #6b7280;
          text-decoration: none;
          padding: 0.375rem 0.625rem;
          background: #f3f4f6;
          border-radius: 6px;
          margin-top: 0.75rem;
          transition: all 0.15s;
          align-self: flex-start;
        }
        .news-grid .story-link:hover {
          background: #2563eb;
          color: white;
        }
        .news-grid .story-link::after {
          content: '›';
          font-size: 1rem;
          font-weight: 600;
        }
        .news-grid .story-footer,
        .news-grid .source-link,
        .news-grid .story-meta,
        .news-grid .story-tag,
        .news-grid .read-more-btn,
        .news-grid .read-more {
          display: none;
        }
      `}</style>
    </div>
  )
}
