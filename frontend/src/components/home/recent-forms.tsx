"use client"

import { BACKEND_URL } from "@/consts"
import { FormType } from "@/types"
import { useSession } from "next-auth/react"
import { useRouter } from "next/navigation"
import { useEffect, useState } from "react"

export default function RecentForms() {

    const { data, status } = useSession()
    const router = useRouter()
    const [forms, setForms] = useState<FormType[]>([])

    useEffect(() => {

        const email = data?.user?.email

        if (!email) return

        (async () => {
            const forms = await (await fetch(`${BACKEND_URL}/Forms?ownerEmail=${email}`)).json()

            setForms(forms)

        })()

    }, [data?.user?.email])

    return (
        <section className="text-center mt-10">
            {
                status === "loading" && <div>Loading</div>
            }

            {
                status === "unauthenticated" && <div>Login to view</div>
            }

            {status === "authenticated" && (forms.length === 0 ? <NoForm /> :
                <div>
                    <div className="flex justify-between items-center">
                        <h4>Recent forms</h4>

                        <div className="flex">
                            <h4>Owned by</h4>
                            <select>
                                <option value="anyone">anyone</option>
                            </select>
                        </div>
                    </div>

                    <div className="grid grid-cols-3 mt-10 gap-6">

                        {
                            forms.map(form => (
                                <div key={form.id} className="h-52 border border-gray-500" onClick={() => router.push(`/${form.id}`)}>
                                    <h4>{form.title}</h4>
                                    {form.ownerEmail}
                                </div>
                            ))
                        }
                    </div>
                </div>
            )}
        </section>
    )
}

const NoForm = () => <>
    <h3>No forms yet</h3>
    <p className="text-gray-500 mt-3">Create a new form to get started</p>
</>