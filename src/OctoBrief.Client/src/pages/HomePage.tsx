import { useState, useRef } from 'react'
import { Octagon, Sparkles } from 'lucide-react'
import { api, PreviewBriefResponse } from '../api'
import {
  TopicSelector,
  CountrySelector,
  LoadingState,
  ErrorState,
  ErrorBanner,
  ResultsView,
} from '../components'
import './HomePage.css'

export function HomePage() {
  const [topic, setTopic] = useState('')
  const [country, setCountry] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [loadingStep, setLoadingStep] = useState('')
  const [result, setResult] = useState<PreviewBriefResponse | null>(null)
  const [error, setError] = useState<string | null>(null)

  // Email send state
  const [email, setEmail] = useState('')
  const [isSendingEmail, setIsSendingEmail] = useState(false)
  const [emailSent, setEmailSent] = useState(false)
  const [emailError, setEmailError] = useState<string | null>(null)
  
  // Ref to prevent double-sending
  const emailSendingRef = useRef(false)

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
      const randomSec = Math.floor(Math.random() * 23) + 40
      setLoadingStep(`Searching for news sources ... ${randomSec}sec`)
      const response = await api.previewBrief(topic, country)

      if (response.success) {
        setResult(response)
      } else {
        setError(response.message || 'Failed to generate brief')
      }
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to generate brief'
      setError(message)
    } finally {
      setIsLoading(false)
      setLoadingStep('')
    }
  }

  const handleSendEmail = async () => {
    // Double-check with ref to prevent any race conditions
    if (!email.trim() || !result?.htmlContent || isSendingEmail || emailSendingRef.current) return
    
    emailSendingRef.current = true
    setIsSendingEmail(true)
    setEmailError(null)

    try {
      const response = await api.sendBriefToEmail(
        email,
        result.subject || 'Your News Brief',
        result.htmlContent
      )
      if (response.success) {
        setEmailSent(true)
      } else {
        setEmailError(response.message || 'Failed to send email')
      }
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to send email'
      setEmailError(message)
    } finally {
      setIsSendingEmail(false)
      emailSendingRef.current = false
    }
  }

  const resetForm = () => {
    setTopic('')
    setCountry('')
    setResult(null)
    setError(null)
    setEmail('')
    setEmailSent(false)
    setEmailError(null)
    emailSendingRef.current = false
  }

  const showForm = !result && !isLoading

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b sticky top-0 z-10">
        <div className="max-w-6xl mx-auto px-4 py-3">
          <div className="flex items-center justify-center">
            <Octagon
              className={`text-primary-600 transition-all duration-500 ${
                isLoading ? 'w-10 h-10 animate-spin-slow' : 'w-8 h-8'
              }`}
            />
          </div>
        </div>
      </header>

      <main className="max-w-6xl mx-auto px-4 py-8">
        {/* Form Section */}
        {showForm && (
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
              {error && <ErrorBanner message={error} />}

              <form onSubmit={handleSubmit} className="space-y-5">
                <TopicSelector selectedTopic={topic} onSelectTopic={setTopic} />

                <CountrySelector selectedCountry={country} onSelectCountry={setCountry} />

                {/* Submit */}
                <button
                  type="submit"
                  disabled={!topic.trim() || !country.trim()}
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
          <LoadingState topic={topic} country={country} loadingStep={loadingStep} />
        )}

        {/* Results Display */}
        {result && result.success && (
          <ResultsView
            result={result}
            topic={topic}
            country={country}
            email={email}
            onEmailChange={setEmail}
            onSendEmail={handleSendEmail}
            isSendingEmail={isSendingEmail}
            emailSent={emailSent}
            emailError={emailError}
            onReset={resetForm}
          />
        )}

        {/* Error State */}
        {result && !result.success && (
          <ErrorState message={result.message || 'Unknown error'} onRetry={resetForm} />
        )}
      </main>
    </div>
  )
}
