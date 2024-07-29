"use client"

import { signIn, useSession } from "next-auth/react";
import { useRouter } from "next/navigation";

export default function CreateFormButton() {

    const router = useRouter()
    const { data, status } = useSession()

    return (<button onClick={async () => {

        if (status === "loading") return

        if (status === "unauthenticated") signIn('google')

        const ownerEmail = data?.user?.email

        if (!ownerEmail) return

        const { createFormId } = await import("@/actions")

        const formId = await createFormId(ownerEmail)
        router.push(`/${formId}`)
    }
    } className="fixed bottom-10 right-10 text-5xl font-light rounded-full border border-black size-11 flex justify-center items-center">
        +
    </button>)
}