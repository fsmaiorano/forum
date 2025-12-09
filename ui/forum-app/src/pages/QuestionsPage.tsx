import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { useAuth } from '@/contexts/AuthContext'
import { questionService, type QuestionDto, type AnswerDto } from '@/lib/question-service'

export function QuestionsPage() {
  const navigate = useNavigate()
  const { user, logout } = useAuth()
  const [questions, setQuestions] = useState<QuestionDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [expandedQuestion, setExpandedQuestion] = useState<string | null>(null)
  const [answers, setAnswers] = useState<Record<string, AnswerDto[]>>({})
  const [answerContents, setAnswerContents] = useState<Record<string, string>>({})
  const [submittingAnswer, setSubmittingAnswer] = useState(false)

  useEffect(() => {
    fetchQuestions()
  }, [])

  const fetchQuestions = async () => {
    try {
      const response = await questionService.getQuestions()
      setQuestions(response.questions)
    } catch (err) {
      setError('Failed to load questions')
      console.error('Error fetching questions:', err)
    } finally {
      setLoading(false)
    }
  }

  const fetchAnswers = async (questionId: string) => {
    try {
      const response = await questionService.getAnswers(questionId)
      setAnswers(prev => ({ ...prev, [questionId]: response.answers }))
    } catch (err) {
      console.error('Error fetching answers:', err)
    }
  }

  const handleShowAnswers = (questionId: string) => {
    if (expandedQuestion === questionId) {
      setExpandedQuestion(null)
    } else {
      setExpandedQuestion(questionId)
      if (!answers[questionId]) {
        fetchAnswers(questionId)
      }
    }
  }

  const handleSubmitAnswer = async (questionId: string) => {
    if (!user || !answerContents[questionId]?.trim()) return

    setSubmittingAnswer(true)
    try {
      await questionService.createAnswer({
        content: answerContents[questionId],
        authorId: user.userId,
        questionId,
      })
      setAnswerContents(prev => ({ ...prev, [questionId]: '' }))
      // Refresh answers
      await fetchAnswers(questionId)
    } catch (err) {
      console.error('Error creating answer:', err)
    } finally {
      setSubmittingAnswer(false)
    }
  }

  const handleLogout = async () => {
    await logout()
    navigate('/login')
  }

  const myQuestions = questions.filter(q => q.authorId === user?.userId)
  const otherQuestions = questions.filter(q => q.authorId !== user?.userId)

  if (loading) {
    return (
      <div className="min-h-screen bg-linear-to-br from-stone-100 to-stone-200 dark:from-stone-900 dark:to-stone-950 flex items-center justify-center">
        <div>Loading...</div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-linear-to-br from-stone-100 to-stone-200 dark:from-stone-900 dark:to-stone-950">
      <header className="border-b bg-card/50 backdrop-blur-sm sticky top-0 z-10">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <h1 className="text-2xl font-bold">Forum</h1>
          <div className="flex items-center gap-4">
            <span className="text-sm text-muted-foreground">{user?.email}</span>
            <Button onClick={handleLogout} variant="outline" size="sm">
              Logout
            </Button>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        <div className="flex items-center justify-between mb-8">
          <div>
            <h2 className="text-3xl font-bold">Questions</h2>
            <p className="text-muted-foreground mt-1">Browse and ask questions</p>
          </div>
          <Button onClick={() => navigate('/questions/create')} size="lg">
            Ask Question
          </Button>
        </div>

        {error && (
          <div className="text-red-600 mb-4">{error}</div>
        )}

        {/* My Questions */}
        {myQuestions.length > 0 && (
          <section className="mb-8">
            <h3 className="text-2xl font-semibold mb-4">My Questions</h3>
            <div className="grid gap-4">
              {myQuestions.map(question => (
                <QuestionCard
                  key={question.id}
                  question={question}
                  isExpanded={expandedQuestion === question.id}
                  answers={answers[question.id] || []}
                  onShowAnswers={() => handleShowAnswers(question.id)}
                  onSubmitAnswer={(content) => handleSubmitAnswer(question.id)}
                  answerContent={answerContents[question.id] || ''}
                  setAnswerContent={(content) => setAnswerContents(prev => ({ ...prev, [question.id]: content }))}
                  submittingAnswer={submittingAnswer}
                  user={user}
                />
              ))}
            </div>
          </section>
        )}

        {/* All Other Questions */}
        {otherQuestions.length > 0 && (
          <section>
            <h3 className="text-2xl font-semibold mb-4">All Questions</h3>
            <div className="grid gap-4">
              {otherQuestions.map(question => (
                <QuestionCard
                  key={question.id}
                  question={question}
                  isExpanded={expandedQuestion === question.id}
                  answers={answers[question.id] || []}
                  onShowAnswers={() => handleShowAnswers(question.id)}
                  onSubmitAnswer={(content) => handleSubmitAnswer(question.id)}
                  answerContent={answerContents[question.id] || ''}
                  setAnswerContent={(content) => setAnswerContents(prev => ({ ...prev, [question.id]: content }))}
                  submittingAnswer={submittingAnswer}
                  user={user}
                />
              ))}
            </div>
          </section>
        )}

        {questions.length === 0 && !loading && (
          <Card>
            <CardHeader>
              <CardTitle>No questions yet</CardTitle>
              <CardDescription>
                Be the first to ask a question! Click the "Ask Question" button above.
              </CardDescription>
            </CardHeader>
          </Card>
        )}
      </main>
    </div>
  )
}

interface QuestionCardProps {
  question: QuestionDto
  isExpanded: boolean
  answers: AnswerDto[]
  onShowAnswers: () => void
  onSubmitAnswer: (content: string) => void
  answerContent: string
  setAnswerContent: (content: string) => void
  submittingAnswer: boolean
  user: any
}

function QuestionCard({
  question,
  isExpanded,
  answers,
  onShowAnswers,
  onSubmitAnswer,
  answerContent,
  setAnswerContent,
  submittingAnswer,
  user
}: QuestionCardProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{question.title}</CardTitle>
        <CardDescription>
          Asked by {question.authorId} on {new Date(question.createdAt).toLocaleDateString()}
        </CardDescription>
      </CardHeader>
      <CardContent>
        <p className="mb-4">{question.content}</p>
        <div className="flex gap-2">
          <Button onClick={onShowAnswers} variant="outline">
            {isExpanded ? 'Hide Answers' : 'Show Answers'} ({answers.length})
          </Button>
        </div>
        {isExpanded && (
          <div className="mt-4">
            <div className="space-y-4 mb-4">
              {answers.map(answer => (
                <div key={answer.id} className="border-l-2 border-muted pl-4">
                  <p>{answer.content}</p>
                  <small className="text-muted-foreground">
                    Answered by {answer.authorId} on {new Date(answer.createdAt).toLocaleDateString()}
                  </small>
                </div>
              ))}
            </div>
            <div className="space-y-2">
              <Label htmlFor={`answer-${question.id}`}>Your Answer</Label>
              <Textarea
                id={`answer-${question.id}`}
                placeholder="Write your answer..."
                value={answerContent}
                onChange={(e) => setAnswerContent(e.target.value)}
              />
              <Button
                onClick={() => onSubmitAnswer(answerContent)}
                disabled={submittingAnswer || !answerContent.trim()}
              >
                {submittingAnswer ? 'Submitting...' : 'Submit Answer'}
              </Button>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  )
}
