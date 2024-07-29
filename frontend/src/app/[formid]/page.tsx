import FormPageContent from "@/components/formPage/form-page-content";
import FormPageHeader from "@/components/formPage/formPageHeader";
import { BACKEND_URL } from "@/consts";
import { FormType } from "@/types";

export default async function Page({ params }: { params: { formid: string } }) {

    const response = await fetch(`${BACKEND_URL}/Forms?formId=${params.formid}`, {
        cache: "no-store"
    })
    const formInfo: FormType = await response.json()

    return <div className="inset-0 relative">
        <FormPageHeader title={formInfo.title} />
        <FormPageContent formInfo={formInfo} />
    </div>
}