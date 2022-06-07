
import './custom.css'
import { Layout } from './Layout';
import { Route } from 'react-router-dom';
import { HomePage } from './components/HomePage';
import Stage from './components/models/WizardModels';
import React from 'react';

export default function App() {

    const [currentStage, setCurrentStage] = React.useState<Stage.Stage>(Stage.Stage.SiteSelection);

    const setNavStage = (stage: Stage.Stage) => {
        setCurrentStage(stage);
    }

    return (
        <Layout stage={currentStage}>
            <Route exact path='/' render={() => <HomePage wizardStageChange={(stage: Stage.Stage) => setNavStage(stage)} />} />
        </Layout>
    );
    
}
