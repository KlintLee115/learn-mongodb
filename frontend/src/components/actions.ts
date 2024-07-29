"use server"

import { revalidatePath } from "next/cache";

export async function serverRevalidatePath(route: string) {
    revalidatePath(route, 'page')
}