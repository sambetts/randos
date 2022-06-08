
import './custom.css'
import { Layout } from './Layout';
import { Route } from 'react-router-dom';
import { HomePage } from './components/HomePage';
import { Stage } from './components/models/enums';
import React from 'react';
import { DeployGuide } from 'components/DeployGuide';
import { Contact } from 'components/Contact';

export default function App() {

    const [currentStage, setCurrentStage] = React.useState<Stage>(Stage.SiteSelection);

    const setNavStage = (stage: Stage) => {
        setCurrentStage(stage);
    }

    return (
        <Layout stage={currentStage}>
            <Route exact path='/' render={() => <HomePage wizardStageChange={(stage: Stage) => setNavStage(stage)} />} />
            <Route exact path='/DeployGuide' render={() => <DeployGuide />} />
            <Route exact path='/Contact' render={() => <Contact />} />
        </Layout>
    );
    
}
