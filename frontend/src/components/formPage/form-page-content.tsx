"use client"

import { Dispatch, SetStateAction, useState } from "react";
import { serverRevalidatePath } from "../actions";
import { BACKEND_URL } from "@/consts";
import { FormType, QuestionChoiceType, QuestionsType } from "@/types";
import Card from "./card";
import { addQuestion } from "@/actions";

export default function FormPageContent({ formInfo }: { formInfo: FormType }) {

    const [description, setDescription] = useState(formInfo.description)
    const [questions, setQuestions] = useState<QuestionsType>(new Map(Object.entries(formInfo.questions)));
    const [title, setTitle] = useState(formInfo.title ?? "Untitled form")

    const debounce = (func: (...args: any[]) => void, timeout = 1500) => {
        let timer: NodeJS.Timeout;
        return (...args: any[]) => {
            clearTimeout(timer);
            timer = setTimeout(() => func(...args), timeout);
        };
    }

    const updateFormField = debounce(async (field: string, value: string) => {

        try {
            const response = await fetch(`${BACKEND_URL}/Forms`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ formId: formInfo.id, field, value })
            });

            if (!response.ok) {
                console.error(response.statusText);
            }

            serverRevalidatePath('/[formid]')

        } catch (error) {
            console.error('Error updating form field:', error);
        }
    })

    const handleFieldChange = (
        e: React.ChangeEvent<HTMLInputElement>,
        setField: Dispatch<SetStateAction<string>>,
        fieldName: string) => {
        setField(e.target.value);
        updateFormField(fieldName, e.target.value);
    }

    const handleQuestionInfoChange = async (qId: string, e: React.ChangeEvent<HTMLInputElement>) => {
        setQuestions(prevQuestions => {

            const updatedQuestions = new Map(prevQuestions);

            const question = updatedQuestions.get(qId)
            if (question) {
                question.title = e.target.value
                updatedQuestions.set(qId, question)
            }

            return updatedQuestions;
        })

        try {
            const response = await fetch(`${BACKEND_URL}/Questions/title`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    formId: formInfo.id,
                    questionId: qId,
                    title: e.target.value
                })
            });

            if (!response.ok) {
                console.error(response.statusText);
            }

            serverRevalidatePath('/[formid]')

        } catch (error) {
            console.error('Error updating form field:', error);
        }

    }

    const changeQuestionChoiceType = async (qId: string, e: React.ChangeEvent<HTMLSelectElement>) => {
        try {

            const value = e.target.value as QuestionChoiceType

            setQuestions(prevQuestions => {

                const updatedQuestions = new Map(prevQuestions);

                const question = updatedQuestions.get(qId)
                if (question) {
                    question.choiceType = value
                    updatedQuestions.set(qId, question)
                }

                return updatedQuestions;
            })

            const response = await fetch(`${BACKEND_URL}/Questions/type`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    formId: formInfo.id,
                    questionId: qId,
                    newType: value
                })
            });

            if (!response.ok) {
                console.error(response.statusText);
            }

            serverRevalidatePath('/[formid]')

        } catch (error) {
            console.error('Error updating form field:', error);
        }
    }

    return (
        <main className="mt-10 border-t-[10px] border-purple-800 rounded-lg pt-5 flex flex-col gap-7">
            <Card cssProps="flex flex-col gap-5">
                <>
                    <input onChange={e => handleFieldChange(e, setTitle, "title")} value={title} className="text-3xl" />
                    <input onChange={e => handleFieldChange(e, setDescription, "description")} value={description} type="text" placeholder="Form description" className="placeholder-slate-500 outline-none" />
                </>
            </Card>

            <section>
                {Array.from(questions.entries()).map(([key, value]) => {
                    const { title, choiceType, options } = value;

                    return (
                        <Card key={key}>
                            {choiceType ? (
                                <>
                                    <div className="flex space-x-2">
                                        <input
                                            onChange={e => handleQuestionInfoChange(key, e)}
                                            className="mb-4 flex-grow"
                                            type="text"
                                            value={title}
                                        />
                                        <select name={key} id="" onChange={e => changeQuestionChoiceType(key, e)} value={choiceType}>
                                            <option value="single">Single choice</option>
                                            <option value="multi">Multiple choice</option>
                                        </select>
                                    </div>
                                    {options && options.length > 0 ? (
                                        options.map(option => (
                                            <label key={option} className="block">
                                                <input
                                                    className="mr-3"
                                                    name={key}
                                                    type={choiceType === "single" ? "radio" : "checkbox"}
                                                    value={option}
                                                />
                                                {option}
                                            </label>
                                        ))
                                    ) : (
                                        <label className="block">
                                            <input
                                                className="mr-3"
                                                name={key}
                                                type={choiceType === "single" ? "radio" : "checkbox"}
                                                value="Untitled"
                                            />
                                            Untitled
                                        </label>
                                    )}
                                </>
                            ) : (
                                <>
                                    <input
                                        onChange={e => handleQuestionInfoChange(key, e)}
                                        className="mb-4 block w-full"
                                        type="text"
                                        value={title}
                                    />
                                    <input
                                        placeholder="Enter your answer here"
                                        className="placeholder-slate-500 outline-none"
                                    />
                                </>
                            )}
                        </Card>
                    );
                })}
            </section>

            <Footer />
        </main>
    )

    function Footer() {

        return <footer className="fixed w-full h-5 bottom-10 flex justify-evenly text-3xl">
            <button onClick={() => addQuestion(formInfo.id, "single")}>S</button>
            <button onClick={() => addQuestion(formInfo.id, "multi")}>M</button>
            <button onClick={() => addQuestion(formInfo.id)}>T</button>
        </footer>
    }
}