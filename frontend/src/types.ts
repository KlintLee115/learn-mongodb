export type FormType = {
    id: string,
    description: string,
    ownerEmail: string,
    title: string,
    questions: QuestionsType
}

export type QuestionsType = Map<string, QuestionInfoType>

export type QuestionInfoType = {
    title: string,
} & (
        { options: string[], choiceType: QuestionChoiceType } |
        { options?: never, choiceType?: never }
    )

export type QuestionChoiceType = "single" | "multi"