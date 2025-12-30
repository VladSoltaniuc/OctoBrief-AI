import { useState, useEffect } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import { Octagon, ArrowLeft } from 'lucide-react'

export function DashboardPage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const [email, setEmail] = useState(searchParams.get('email') || '')

  useEffect(() => {
    if (!email) {
      const userEmail = prompt('Enter your email to view your dashboard:')
      if (!userEmail) {
        navigate('/')
        return
      }
      setEmail(userEmail)
    }
  }, [email, navigate])

  if (!email) return null

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
    </div>
  )
}
