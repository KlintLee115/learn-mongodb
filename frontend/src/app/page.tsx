import CreateFormButton from "@/components/home/create-form-button";
import Header from "@/components/home/home-header"
import RecentForms from "@/components/home/recent-forms";

export default function Home() {
  return (
    <>
      <Header />
      <RecentForms />
      <CreateFormButton />
    </>
  );
}
