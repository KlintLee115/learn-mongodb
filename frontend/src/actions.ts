import { BACKEND_URL } from "./consts";
import { QuestionChoiceType } from "./types";

export async function createFormId(userEmail: string): Promise<string> {
    try {
        const response = await fetch('http://localhost:5242/api/Forms', {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                ownerEmail: userEmail
            })
        })

        if (!response.ok) {
            throw new Error(response.statusText);
        }

        const { formId }: { formId: string } = await response.json()

        return formId
    } catch (error) {
        throw error
    }
}

export async function addQuestion(formId: string, choiceType?: QuestionChoiceType) {

    try {
        const requestBody: { formId: string, title: string; qChoiceType?: QuestionChoiceType } =
        {
            title: "Untitled question",
            formId: formId
        }

        if (choiceType) {
            requestBody.qChoiceType = choiceType
        }

        const response = await fetch(`${BACKEND_URL}/Questions`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestBody)
        })
    }

    catch (err) {
        throw err
    }
}