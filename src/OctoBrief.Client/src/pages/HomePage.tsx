import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { Octagon, Sparkles, Globe, Mail, Clock, ArrowRight, TrendingUp, Plus, X, Send } from 'lucide-react'
import { api } from '../api'
import { CreatePreferenceDto, NotificationFrequency } from '../types'

// Extract website name from URL (e.g., "https://www.example.com/path" -> "Example")
function extractWebsiteName(url: string): string {
  try {
    const hostname = new URL(url).hostname
    // Remove www. prefix and get the main domain part
    const parts = hostname.replace(/^www\./, '').split('.')
    // Get the main name (first part before TLD)
    const name = parts[0]
    // Capitalize first letter
    return name.charAt(0).toUpperCase() + name.slice(1)
  } catch {
    return 'Website'
  }
}

export function HomePage() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [websiteUrls, setWebsiteUrls] = useState<string[]>([''])
  const [frequency, setFrequency] = useState<NotificationFrequency>(NotificationFrequency.Daily)
  const [previewUrl, setPreviewUrl] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [sendNow, setSendNow] = useState(false)

  const createMutation = useMutation({
    mutationFn: (data: CreatePreferenceDto) => api.createPreference(data),
  })

  const previewMutation = useMutation({
    mutationFn: (url: string) => api.previewSummary(url),
    onSuccess: (data) => {
      alert(`Preview:\n\n${data.summary}\n\nFound ${data.headlines.length} headlines!`)
    },
    onError: (error: Error) => {
      alert(`❌ ${error.message}`)
    },
  })

  const addWebsiteField = () => {
    setWebsiteUrls([...websiteUrls, ''])
  }

  const removeWebsiteField = (index: number) => {
    if (websiteUrls.length === 1) return
    setWebsiteUrls(websiteUrls.filter((_, i) => i !== index))
  }

  const updateWebsiteUrl = (index: number, value: string) => {
    const updated = [...websiteUrls]
    updated[index] = value
    setWebsiteUrls(updated)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)

    const validUrls = websiteUrls.filter(url => url.trim() !== '')
    if (validUrls.length === 0) {
      alert('Please add at least one website URL')
      setIsSubmitting(false)
      return
    }

    try {
      let successCount = 0
      let failCount = 0
      const errors: string[] = []

      for (const url of validUrls) {
        try {
          const preference = await createMutation.mutateAsync({
            email,
            websiteUrl: url,
            websiteName: extractWebsiteName(url),
            frequency
          })
          successCount++
          
          // Trigger immediate email if sendNow is checked
          if (sendNow && preference.id) {
            try {
              await api.triggerMonitoring(preference.id)
            } catch (triggerError) {
              console.error('Failed to trigger immediate send:', triggerError)
            }
          }
        } catch (error: any) {
          failCount++
          errors.push(`${url}: ${error.message}`)
        }
      }

      if (successCount > 0) {
        alert(`✅ Successfully added ${successCount} website${successCount > 1 ? 's' : ''} to monitoring!${sendNow ? '\n\n📧 Email summary is being generated and will be sent shortly!' : ''}${failCount > 0 ? `\n\n⚠️ ${failCount} failed:\n${errors.join('\n')}` : ''}`)
        navigate(`/dashboard?email=${encodeURIComponent(email)}`)
      } else {
        alert(`❌ Failed to add websites:\n${errors.join('\n')}`)
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  const handlePreview = () => {
    if (!previewUrl) return
    previewMutation.mutate(previewUrl)
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-50 via-white to-blue-50">
      {/* Header */}
      <header className="border-b bg-white/80 backdrop-blur-sm sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <Octagon className="w-8 h-8 text-primary-600" />
              <h1 className="text-2xl font-bold text-gray-900">OctoBrief</h1>
            </div>
            <div className="flex items-center gap-4">
              <button
                onClick={() => navigate('/test')}
                className="text-gray-500 hover:text-gray-700 font-medium text-sm"
              >
                🔧 Debug
              </button>
              <button
                onClick={() => {
                  const email = prompt('Enter your email to view dashboard:')
                  if (email) navigate(`/dashboard?email=${encodeURIComponent(email)}`)
                }}
                className="text-primary-600 hover:text-primary-700 font-medium flex items-center gap-2"
              >
                My Dashboard
                <ArrowRight className="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 px-4 py-2 bg-primary-100 text-primary-700 rounded-full text-sm font-medium mb-6">
            <Sparkles className="w-4 h-4" />
            AI-Powered Web Monitoring
          </div>
          <h2 className="text-5xl font-bold text-gray-900 mb-6">
            Stay Updated,<br />Without the Overwhelm
          </h2>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Monitor any website and receive AI-summarized updates directly to your inbox.
            Never miss important news again.
          </p>
        </div>

        {/* Features */}
        <div className="grid md:grid-cols-3 gap-8 mb-16">
          <div className="bg-white rounded-2xl p-8 shadow-sm border border-gray-100">
            <Globe className="w-12 h-12 text-primary-600 mb-4" />
            <h3 className="text-xl font-semibold mb-3">Monitor Any Website</h3>
            <p className="text-gray-600">
              Track news sites, blogs, social media pages, or any web content you care about.
            </p>
          </div>
          <div className="bg-white rounded-2xl p-8 shadow-sm border border-gray-100">
            <TrendingUp className="w-12 h-12 text-primary-600 mb-4" />
            <h3 className="text-xl font-semibold mb-3">AI Summarization</h3>
            <p className="text-gray-600">
              Get concise summaries of the main headlines, saving you time and effort.
            </p>
          </div>
          <div className="bg-white rounded-2xl p-8 shadow-sm border border-gray-100">
            <Mail className="w-12 h-12 text-primary-600 mb-4" />
            <h3 className="text-xl font-semibold mb-3">Email Delivery</h3>
            <p className="text-gray-600">
              Receive updates daily, weekly, or monthly - your choice, your schedule.
            </p>
          </div>
        </div>

        {/* Main Form */}
        <div className="max-w-4xl mx-auto">
          <div className="bg-white rounded-3xl shadow-xl border border-gray-100 p-8 md:p-12">
            <h3 className="text-2xl font-bold text-gray-900 mb-8 text-center">
              Start Monitoring Now
            </h3>
            
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Mail className="w-4 h-4 inline mr-2" />
                  Your Email
                </label>
                <input
                  type="email"
                  required
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="you@example.com"
                  className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none transition"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Globe className="w-4 h-4 inline mr-2" />
                  Website URLs to Monitor
                </label>
                <div className="space-y-3">
                  {websiteUrls.map((url, index) => (
                    <div key={index} className="flex gap-2">
                      <input
                        type="url"
                        required
                        value={url}
                        onChange={(e) => updateWebsiteUrl(index, e.target.value)}
                        placeholder="https://example.com"
                        className="flex-1 px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none transition"
                      />
                      {websiteUrls.length > 1 && (
                        <button
                          type="button"
                          onClick={() => removeWebsiteField(index)}
                          className="p-3 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-xl transition"
                          title="Remove"
                        >
                          <X className="w-5 h-5" />
                        </button>
                      )}
                    </div>
                  ))}
                  <button
                    type="button"
                    onClick={addWebsiteField}
                    className="w-full flex items-center justify-center gap-2 px-4 py-3 text-sm bg-gray-50 text-gray-700 hover:bg-gray-100 border border-gray-200 rounded-xl transition font-medium"
                  >
                    <Plus className="w-4 h-4" />
                    Add Another Website
                  </button>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Clock className="w-4 h-4 inline mr-2" />
                  Notification Frequency
                </label>
                <select
                  value={frequency}
                  onChange={(e) => setFrequency(parseInt(e.target.value) as NotificationFrequency)}
                  className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none transition"
                >
                  <option value={NotificationFrequency.Daily}>Daily</option>
                  <option value={NotificationFrequency.Weekly}>Weekly (Mondays)</option>
                  <option value={NotificationFrequency.Monthly}>Monthly (1st of month)</option>
                </select>
              </div>

              <div className="flex items-center gap-3 p-4 bg-primary-50 rounded-xl border border-primary-100">
                <input
                  type="checkbox"
                  id="sendNow"
                  checked={sendNow}
                  onChange={(e) => setSendNow(e.target.checked)}
                  className="w-5 h-5 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
                />
                <label htmlFor="sendNow" className="flex items-center gap-2 text-sm font-medium text-gray-700 cursor-pointer">
                  <Send className="w-4 h-4 text-primary-600" />
                  Send email summary immediately after adding
                </label>
              </div>

              <button
                type="submit"
                disabled={isSubmitting}
                className="w-full bg-primary-600 hover:bg-primary-700 text-white font-semibold py-4 rounded-xl transition disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              >
                {isSubmitting ? (
                  <>
                    <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                    Adding Websites...
                  </>
                ) : (
                  <>
                    <Sparkles className="w-5 h-5" />
                    Start Monitoring
                  </>
                )}
              </button>
            </form>

            {/* Preview Section */}
            <div className="mt-8 pt-8 border-t border-gray-200">
              <p className="text-sm text-gray-600 mb-4 text-center">
                Want to test it first? Preview how AI will summarize a website:
              </p>
              <div className="flex gap-2">
                <input
                  type="url"
                  value={previewUrl}
                  onChange={(e) => setPreviewUrl(e.target.value)}
                  placeholder="https://example.com"
                  className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none"
                />
                <button
                  type="button"
                  onClick={handlePreview}
                  disabled={previewMutation.isPending || !previewUrl}
                  className="px-6 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 font-medium rounded-lg transition disabled:opacity-50"
                >
                  {previewMutation.isPending ? 'Loading...' : 'Preview'}
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t bg-white mt-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 text-center text-gray-600 text-sm">
          <p>© 2025 OctoBrief. AI-powered web monitoring made simple.</p>
        </div>
      </footer>
    </div>
  )
}
