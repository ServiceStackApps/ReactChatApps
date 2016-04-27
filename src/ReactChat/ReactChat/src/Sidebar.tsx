import * as React from 'react';
import { reduxify } from './utils';
import { User } from './User';

interface Window {
    nativeHost: any;
}

@reduxify(
    (state) => ({
        users: state.users,
        hideExamples: state.hideExamples
    }),
    (dispatch) => ({
        toggleExamples:() => dispatch({ type: 'EXAMPLES_TOGGLE' })
    })
)
export class Sidebar extends React.Component<any, any> {
    render() {
        const height = this.props.hideExamples ? "25px" : "auto";
        const label = this.props.hideExamples ? "show" : "hide";

        const isWinforms = window.nativeHost.platform === "winforms";
        const isMac = window.nativeHost.platform === "mac";

        return (
            <div id="right">
                <div id="users">
                    {this.props.users.map(user => <User key={user.userId} user={user} onSelect={() => this.props.onUserSelected(user)} />)}
                </div>
                <div id="examples" style={{ height: height }}>
                    <span style={{position: "absolute", top: "2px", right: "7px"}} onClick={this.props.toggleExamples}>{label}</span>
                    <span onClick={e => this.props.onCommandSelected((e.target as HTMLElement).innerHTML)}>
                        <h4><a href="#">Example Commands</a></h4>
                        {isWinforms ? <div>/cmd.toggleFormBorder</div> : null}
                        <div>/cmd.announce This is your captain speaking ...</div>
                        <div>/cmd.toggle$#channels</div>
                        <h4><a href="#">CSS</a></h4>
                        {isMac ? null : <div>/css.background-image url(http://bit.ly/1yIJOBH)</div>}
                        <div>/css.background-image url(/img/disco.jpg)</div>
                        <div>@me /css.background$#top #673ab7</div>
                        <div>/css.background$#bottom #0091ea</div>
                        <div>/css.background$#right #fffde7</div>
                        <div>/css.color$#welcome #ff0</div>
                        <div>/css.visibility$img,a hidden</div>
                        <div>/css.visibility$img,a visible</div>
                        <h4><a href="#">Receivers</a></h4>
                        <div>/tv.watch https://youtu.be/9bZkp7q19f0</div>
                        <div>/tv.watch https://servicestack.net/img/logo-220.png</div>
                        <div>@me /tv.off</div>
                        {isWinforms ? <div>/windows.shrink</div> : null}
                        {isWinforms ? <div>/windows.grow</div> : null}
                        {isWinforms ? <div>/windows.dance</div> : null}
                        <div>/window.location http://google.com</div>
                        <h4><a href="#">Triggers</a></h4>
                        <div>/trigger.customEvent arg</div>
                    </span>
                </div>
            </div>
        );
    }
}

